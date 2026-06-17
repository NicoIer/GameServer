using System.Collections.Concurrent;

namespace GameServer.Core.Fibers;

public enum FiberSchedulerType
{
    Main,
    Thread,
    ThreadPool,
}

public readonly record struct FiberAddress(int FiberId, string Name);

public readonly record struct FiberUpdateContext(Fiber Fiber, long TimeNowMs);

public interface IFiberModule
{
    ValueTask OnStartAsync(Fiber fiber, CancellationToken cancellationToken);
    void OnUpdate(FiberUpdateContext context);
    void OnLateUpdate(FiberUpdateContext context);
    ValueTask OnStopAsync(CancellationToken cancellationToken);
}

public interface IFiberScheduler : IDisposable
{
    void Add(Fiber fiber);
    void Remove(int fiberId);
    void Wake(Fiber fiber);
}

public sealed class FiberSynchronizationContext : SynchronizationContext
{
    private readonly ConcurrentQueue<FiberCallback> _queue = new();
    private readonly Action _wake;

    public FiberSynchronizationContext(Action wake)
    {
        _wake = wake;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Enqueue(new FiberCallback(d, state));
        _wake();
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        if (ReferenceEquals(System.Threading.SynchronizationContext.Current, this))
        {
            d(state);
            return;
        }

        using var wait = new ManualResetEventSlim();
        Exception? exception = null;
        Post(x =>
        {
            try
            {
                d(x);
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                wait.Set();
            }
        }, state);

        wait.Wait();
        if (exception != null)
        {
            throw exception;
        }
    }

    public int Drain(int maxCount = 1024)
    {
        int count = 0;
        while (count < maxCount && _queue.TryDequeue(out FiberCallback callback))
        {
            callback.Invoke();
            count++;
        }

        return count;
    }

    public void Cancel()
    {
        while (_queue.TryDequeue(out FiberCallback callback))
        {
            callback.Cancel();
        }
    }

    private readonly struct FiberCallback
    {
        private readonly SendOrPostCallback _callback;
        private readonly object? _state;

        public FiberCallback(SendOrPostCallback callback, object? state)
        {
            _callback = callback;
            _state = state;
        }

        public void Invoke()
        {
            _callback(_state);
        }

        public void Cancel()
        {
            if (_state is IFiberCancellable cancellable)
            {
                cancellable.Cancel();
            }
        }
    }
}

internal interface IFiberCancellable
{
    void Cancel();
}

public sealed class Fiber
{
    [ThreadStatic]
    private static Fiber? s_current;

    private readonly IFiberModule _module;
    private readonly Action<Fiber> _wake;
    private readonly CancellationTokenSource _shutdown = new();
    private int _updating;
    private int _stopped;

    internal Fiber(int fiberId, string name, IFiberModule module, Action<Fiber> wake)
    {
        FiberId = fiberId;
        Name = name;
        _module = module;
        _wake = wake;
        SynchronizationContext = new FiberSynchronizationContext(() => _wake(this));
    }

    public static Fiber? Current => s_current;
    public int FiberId { get; }
    public string Name { get; }
    public long NextWakeTimeMs { get; set; }
    public FiberAddress Address => new(FiberId, Name);
    public FiberSynchronizationContext SynchronizationContext { get; }
    public bool IsStopped => Volatile.Read(ref _stopped) != 0;

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        await _module.OnStartAsync(this, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask StopAsync()
    {
        if (Interlocked.Exchange(ref _stopped, 1) != 0)
        {
            return;
        }

        _shutdown.Cancel();
        SynchronizationContext.Cancel();

        await _module.OnStopAsync(CancellationToken.None).ConfigureAwait(false);
        _shutdown.Dispose();
    }

    public void Post(Action action)
    {
        if (IsStopped)
        {
            return;
        }

        SynchronizationContext.Post(_ => action(), null);
    }

    public Task PostAsync(Action action)
    {
        if (IsStopped)
        {
            return Task.FromCanceled(new CancellationToken(true));
        }

        var completion = new FiberCompletion<bool>();
        SynchronizationContext.Post(_ =>
        {
            try
            {
                action();
                completion.SetResult(true);
            }
            catch (Exception e)
            {
                completion.SetException(e);
            }
        }, completion);

        return completion.Task;
    }

    public Task<T> CallAsync<T>(Func<T> action)
    {
        if (IsStopped)
        {
            return Task.FromCanceled<T>(new CancellationToken(true));
        }

        var completion = new FiberCompletion<T>();
        SynchronizationContext.Post(_ =>
        {
            try
            {
                completion.SetResult(action());
            }
            catch (Exception e)
            {
                completion.SetException(e);
            }
        }, completion);

        return completion.Task;
    }

    internal bool RunOnce(long timeNowMs)
    {
        if (IsStopped || Interlocked.Exchange(ref _updating, 1) != 0)
        {
            return false;
        }

        Fiber? previousFiber = s_current;
        System.Threading.SynchronizationContext? previousContext = System.Threading.SynchronizationContext.Current;
        try
        {
            s_current = this;
            System.Threading.SynchronizationContext.SetSynchronizationContext(SynchronizationContext);
            var context = new FiberUpdateContext(this, timeNowMs);
            SynchronizationContext.Drain();
            _module.OnUpdate(context);
            _module.OnLateUpdate(context);
            SynchronizationContext.Drain();
            return true;
        }
        finally
        {
            System.Threading.SynchronizationContext.SetSynchronizationContext(previousContext);
            s_current = previousFiber;
            Volatile.Write(ref _updating, 0);
        }
    }

    private sealed class FiberCompletion<T> : IFiberCancellable
    {
        private readonly TaskCompletionSource<T> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<T> Task => _completion.Task;

        public void SetResult(T result)
        {
            _completion.TrySetResult(result);
        }

        public void SetException(Exception exception)
        {
            _completion.TrySetException(exception);
        }

        public void Cancel()
        {
            _completion.TrySetCanceled();
        }
    }
}

public sealed class FiberManager : IDisposable
{
    private readonly Dictionary<FiberSchedulerType, IFiberScheduler> _schedulers = new();
    private readonly ConcurrentDictionary<int, Fiber> _fibers = new();
    private int _nextFiberId;
    private bool _disposed;

    public FiberManager(int threadPoolWorkerCount = 0)
    {
        _schedulers[FiberSchedulerType.Main] = new MainFiberScheduler();
        _schedulers[FiberSchedulerType.Thread] = new ThreadFiberScheduler();
        _schedulers[FiberSchedulerType.ThreadPool] = new ThreadPoolFiberScheduler(threadPoolWorkerCount);
    }

    public async Task<Fiber> CreateAsync(
        FiberSchedulerType schedulerType,
        string name,
        IFiberModule module,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        int fiberId = Interlocked.Increment(ref _nextFiberId);
        var fiber = new Fiber(fiberId, name, module, Wake);
        if (!_fibers.TryAdd(fiberId, fiber))
        {
            throw new InvalidOperationException($"duplicate fiber id={fiberId}");
        }

        try
        {
            await fiber.StartAsync(cancellationToken);
            IFiberScheduler scheduler = _schedulers[schedulerType];
            scheduler.Add(fiber);
            return fiber;
        }
        catch
        {
            _fibers.TryRemove(fiberId, out _);
            throw;
        }
    }

    public bool TryGet(int fiberId, out Fiber? fiber)
    {
        return _fibers.TryGetValue(fiberId, out fiber);
    }

    public async Task RemoveAsync(int fiberId)
    {
        if (!_fibers.TryRemove(fiberId, out Fiber? fiber))
        {
            return;
        }

        foreach (IFiberScheduler scheduler in _schedulers.Values)
        {
            scheduler.Remove(fiberId);
        }

        await fiber.StopAsync();
    }

    public void UpdateMain(long timeNowMs)
    {
        if (_schedulers[FiberSchedulerType.Main] is MainFiberScheduler scheduler)
        {
            scheduler.Update(timeNowMs);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        foreach (Fiber fiber in _fibers.Values)
        {
            fiber.StopAsync().AsTask().GetAwaiter().GetResult();
        }

        foreach (IFiberScheduler scheduler in _schedulers.Values)
        {
            scheduler.Dispose();
        }

        _fibers.Clear();
        _schedulers.Clear();
    }

    private void Wake(Fiber fiber)
    {
        foreach (IFiberScheduler scheduler in _schedulers.Values)
        {
            scheduler.Wake(fiber);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FiberManager));
        }
    }
}

internal sealed class MainFiberScheduler : IFiberScheduler
{
    private readonly ConcurrentDictionary<int, Fiber> _fibers = new();
    private readonly ConcurrentQueue<int> _ready = new();

    public void Add(Fiber fiber)
    {
        _fibers[fiber.FiberId] = fiber;
        Wake(fiber);
    }

    public void Remove(int fiberId)
    {
        _fibers.TryRemove(fiberId, out _);
    }

    public void Wake(Fiber fiber)
    {
        if (_fibers.ContainsKey(fiber.FiberId))
        {
            _ready.Enqueue(fiber.FiberId);
        }
    }

    public void Update(long timeNowMs)
    {
        foreach (Fiber fiber in _fibers.Values)
        {
            if (!fiber.IsStopped && fiber.NextWakeTimeMs <= timeNowMs)
            {
                _ready.Enqueue(fiber.FiberId);
            }
        }

        int count = 0;
        while (count < 4096 && _ready.TryDequeue(out int fiberId))
        {
            if (_fibers.TryGetValue(fiberId, out Fiber? fiber))
            {
                fiber.RunOnce(timeNowMs);
                if (!fiber.IsStopped && fiber.NextWakeTimeMs <= timeNowMs)
                {
                    _ready.Enqueue(fiber.FiberId);
                }
            }

            count++;
        }
    }

    public void Dispose()
    {
        _fibers.Clear();
    }
}

internal sealed class ThreadFiberScheduler : IFiberScheduler
{
    private readonly ConcurrentDictionary<int, ThreadFiberRunner> _runners = new();

    public void Add(Fiber fiber)
    {
        var runner = new ThreadFiberRunner(fiber);
        if (_runners.TryAdd(fiber.FiberId, runner))
        {
            runner.Start();
        }
    }

    public void Remove(int fiberId)
    {
        if (_runners.TryRemove(fiberId, out ThreadFiberRunner? runner))
        {
            runner.Dispose();
        }
    }

    public void Wake(Fiber fiber)
    {
        if (_runners.TryGetValue(fiber.FiberId, out ThreadFiberRunner? runner))
        {
            runner.Wake();
        }
    }

    public void Dispose()
    {
        foreach (ThreadFiberRunner runner in _runners.Values)
        {
            runner.Dispose();
        }

        _runners.Clear();
    }

    private sealed class ThreadFiberRunner : IDisposable
    {
        private readonly Fiber _fiber;
        private readonly AutoResetEvent _wakeSignal = new(false);
        private readonly CancellationTokenSource _shutdown = new();
        private readonly Thread _thread;

        public ThreadFiberRunner(Fiber fiber)
        {
            _fiber = fiber;
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = $"Fiber-{fiber.FiberId}-{fiber.Name}",
            };
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Wake()
        {
            _wakeSignal.Set();
        }

        public void Dispose()
        {
            _shutdown.Cancel();
            _wakeSignal.Set();
            _thread.Join();
            _wakeSignal.Dispose();
            _shutdown.Dispose();
        }

        private void Run()
        {
            while (!_shutdown.IsCancellationRequested && !_fiber.IsStopped)
            {
                long now = Environment.TickCount64;
                if (_fiber.NextWakeTimeMs <= now)
                {
                    _fiber.RunOnce(now);
                    now = Environment.TickCount64;
                }

                int sleepMs = FiberSchedulerSleep.GetSleepMs(now, _fiber.NextWakeTimeMs);
                _wakeSignal.WaitOne(sleepMs);
            }
        }
    }
}

internal sealed class ThreadPoolFiberScheduler : IFiberScheduler
{
    private readonly ConcurrentDictionary<int, Fiber> _fibers = new();
    private readonly ConcurrentDictionary<int, byte> _readySet = new();
    private readonly ConcurrentQueue<int> _ready = new();
    private readonly PriorityQueue<int, long> _delayQueue = new();
    private readonly object _delayLock = new();
    private readonly AutoResetEvent _wakeSignal = new(false);
    private readonly CancellationTokenSource _shutdown = new();
    private readonly List<Thread> _workers = new();

    public ThreadPoolFiberScheduler(int workerCount)
    {
        int count = workerCount > 0 ? workerCount : Math.Max(1, Environment.ProcessorCount / 2);
        for (int i = 0; i < count; i++)
        {
            var thread = new Thread(Run)
            {
                IsBackground = true,
                Name = $"FiberPool-{i}",
            };
            _workers.Add(thread);
            thread.Start();
        }
    }

    public void Add(Fiber fiber)
    {
        _fibers[fiber.FiberId] = fiber;
        Wake(fiber);
    }

    public void Remove(int fiberId)
    {
        _fibers.TryRemove(fiberId, out _);
        _readySet.TryRemove(fiberId, out _);
    }

    public void Wake(Fiber fiber)
    {
        if (_fibers.ContainsKey(fiber.FiberId) && _readySet.TryAdd(fiber.FiberId, 0))
        {
            _ready.Enqueue(fiber.FiberId);
            _wakeSignal.Set();
        }
    }

    public void Dispose()
    {
        _shutdown.Cancel();
        _wakeSignal.Set();
        foreach (Thread worker in _workers)
        {
            worker.Join();
        }

        _wakeSignal.Dispose();
        _shutdown.Dispose();
        _fibers.Clear();
        _readySet.Clear();
    }

    private void Run()
    {
        while (!_shutdown.IsCancellationRequested)
        {
            long now = Environment.TickCount64;
            MoveDueFibers(now);

            if (_ready.TryDequeue(out int fiberId))
            {
                _readySet.TryRemove(fiberId, out _);
                if (_fibers.TryGetValue(fiberId, out Fiber? fiber) && !fiber.IsStopped)
                {
                    try
                    {
                        fiber.RunOnce(now);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    ScheduleDelay(fiber);
                }

                continue;
            }

            _wakeSignal.WaitOne(FiberSchedulerSleep.GetSleepMs(now, GetNextWakeTime()));
        }
    }

    private void ScheduleDelay(Fiber fiber)
    {
        if (fiber.IsStopped)
        {
            return;
        }

        long wakeTime = fiber.NextWakeTimeMs;
        if (wakeTime <= 0)
        {
            wakeTime = Environment.TickCount64 + 1;
            fiber.NextWakeTimeMs = wakeTime;
        }

        lock (_delayLock)
        {
            _delayQueue.Enqueue(fiber.FiberId, wakeTime);
        }

        _wakeSignal.Set();
    }

    private void MoveDueFibers(long now)
    {
        lock (_delayLock)
        {
            while (_delayQueue.TryPeek(out int fiberId, out long wakeTime) && wakeTime <= now)
            {
                _delayQueue.Dequeue();
                if (_fibers.ContainsKey(fiberId))
                {
                    if (_readySet.TryAdd(fiberId, 0))
                    {
                        _ready.Enqueue(fiberId);
                    }
                }
            }
        }
    }

    private long GetNextWakeTime()
    {
        lock (_delayLock)
        {
            if (_delayQueue.TryPeek(out _, out long wakeTime))
            {
                return wakeTime;
            }
        }

        return 0;
    }
}

internal static class FiberSchedulerSleep
{
    public static int GetSleepMs(long now, long wakeTime)
    {
        if (wakeTime <= 0)
        {
            return 50;
        }

        long diff = wakeTime - now;
        if (diff <= 0)
        {
            return 1;
        }

        return (int)Math.Clamp(diff, 1, 50);
    }
}

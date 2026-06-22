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

internal interface IFiberScheduler : IDisposable
{
    void Add(int fiberId);
    void Remove(int fiberId);
}

public sealed class FiberSynchronizationContext : SynchronizationContext
{
    private readonly ConcurrentQueue<FiberCallback> _queue = new();

    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Enqueue(new FiberCallback(d, state));
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
    private readonly ConcurrentQueue<Action> _nextUpdateQueue = new();
    private readonly CancellationTokenSource _shutdown = new();
    private int _started;
    private int _stopping;
    private int _updating;
    private int _stopped;

    internal Fiber(int fiberId, string name, IFiberModule module)
    {
        FiberId = fiberId;
        Name = name;
        _module = module;
        SynchronizationContext = new FiberSynchronizationContext();
    }

    public static Fiber? Current => s_current;
    public int FiberId { get; }
    public string Name { get; }
    public long NextWakeTimeMs { get; set; }
    public FiberAddress Address => new(FiberId, Name);
    public FiberSynchronizationContext SynchronizationContext { get; }
    public bool IsStopped => Volatile.Read(ref _stopped) != 0;
    private bool IsStopping => Volatile.Read(ref _stopping) != 0;
    private bool IsUnavailable => IsStopping || IsStopped;

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        var completion = new FiberCompletion<bool>();
        SynchronizationContext.Post(_ =>
        {
            _ = ExecuteStartAsync(cancellationToken, completion);
        }, completion);

        await completion.Task.ConfigureAwait(false);
    }

    public async ValueTask StopAsync()
    {
        await StopOnFiberAsync().ConfigureAwait(false);
    }

    public void Post(Action action)
    {
        if (IsUnavailable)
        {
            return;
        }

        SynchronizationContext.Post(_ => action(), null);
    }

    public Task PostAsync(Action action)
    {
        if (IsUnavailable)
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
        if (IsUnavailable)
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

    public Task<T> CallAsync<T>(Func<ValueTask<T>> action)
    {
        if (IsUnavailable)
        {
            return Task.FromCanceled<T>(new CancellationToken(true));
        }

        var completion = new FiberCompletion<T>();
        SynchronizationContext.Post(_ =>
        {
            _ = ExecuteAsync(action, completion);
        }, completion);

        return completion.Task;
    }

    public void PostNextUpdate(Action action)
    {
        if (IsUnavailable)
        {
            return;
        }

        _nextUpdateQueue.Enqueue(action);
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
            DrainNextUpdateQueue();
            SynchronizationContext.Drain();
            if (Volatile.Read(ref _started) != 0 && !IsUnavailable)
            {
                _module.OnUpdate(context);
                _module.OnLateUpdate(context);
            }

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

    internal async Task StopOnFiberAsync()
    {
        if (IsStopped)
        {
            return;
        }

        var completion = new FiberCompletion<bool>();
        SynchronizationContext.Post(_ =>
        {
            _ = ExecuteStopAsync(completion);
        }, completion);

        await completion.Task.ConfigureAwait(false);
    }

    internal async Task StopInlineAsync()
    {
        if (IsStopped)
        {
            return;
        }

        Fiber? previousFiber = s_current;
        System.Threading.SynchronizationContext? previousContext = System.Threading.SynchronizationContext.Current;
        try
        {
            s_current = this;
            System.Threading.SynchronizationContext.SetSynchronizationContext(SynchronizationContext);
            await StopCoreAsync().ConfigureAwait(false);
        }
        finally
        {
            System.Threading.SynchronizationContext.SetSynchronizationContext(previousContext);
            s_current = previousFiber;
        }
    }

    private async Task ExecuteStartAsync(CancellationToken cancellationToken, FiberCompletion<bool> completion)
    {
        try
        {
            await _module.OnStartAsync(this, cancellationToken);
            Volatile.Write(ref _started, 1);
            completion.SetResult(true);
        }
        catch (Exception e)
        {
            completion.SetException(e);
        }
    }

    private async Task ExecuteStopAsync(FiberCompletion<bool> completion)
    {
        try
        {
            await StopCoreAsync();
            completion.SetResult(true);
        }
        catch (Exception e)
        {
            completion.SetException(e);
        }
    }

    private async Task StopCoreAsync()
    {
        if (Interlocked.Exchange(ref _stopping, 1) != 0)
        {
            return;
        }

        _shutdown.Cancel();
        await _module.OnStopAsync(CancellationToken.None);
        SynchronizationContext.Cancel();
        _shutdown.Dispose();
        Volatile.Write(ref _stopped, 1);
    }

    private async Task ExecuteAsync<T>(Func<ValueTask<T>> action, FiberCompletion<T> completion)
    {
        try
        {
            T result = await action();
            completion.SetResult(result);
        }
        catch (Exception e)
        {
            completion.SetException(e);
        }
    }

    private void DrainNextUpdateQueue()
    {
        while (_nextUpdateQueue.TryDequeue(out Action? action))
        {
            action();
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
    private readonly ConcurrentDictionary<int, FiberSchedulerType> _fiberSchedulerTypes = new();
    private int _nextFiberId;
    private bool _disposed;

    public FiberManager(int threadPoolWorkerCount = 0)
    {
        _schedulers[FiberSchedulerType.Main] = new MainFiberScheduler(this);
        _schedulers[FiberSchedulerType.Thread] = new ThreadFiberScheduler(this);
        _schedulers[FiberSchedulerType.ThreadPool] = new ThreadPoolFiberScheduler(this, threadPoolWorkerCount);
    }

    public async Task<Fiber> CreateAsync(
        FiberSchedulerType schedulerType,
        string name,
        IFiberModule module,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        int fiberId = Interlocked.Increment(ref _nextFiberId);
        var fiber = new Fiber(fiberId, name, module);
        if (!_fibers.TryAdd(fiberId, fiber))
        {
            throw new InvalidOperationException($"duplicate fiber id={fiberId}");
        }

        _fiberSchedulerTypes[fiberId] = schedulerType;
        IFiberScheduler scheduler = _schedulers[schedulerType];
        try
        {
            Task startTask = fiber.StartAsync(cancellationToken).AsTask();
            scheduler.Add(fiberId);
            await AwaitFiberTaskAsync(schedulerType, startTask).ConfigureAwait(false);
            return fiber;
        }
        catch
        {
            scheduler.Remove(fiberId);
            _fiberSchedulerTypes.TryRemove(fiberId, out _);
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
        if (!_fibers.TryGetValue(fiberId, out Fiber? fiber))
        {
            return;
        }

        bool hasSchedulerType = _fiberSchedulerTypes.TryGetValue(fiberId, out FiberSchedulerType schedulerType);
        Task stopTask = fiber.StopOnFiberAsync();
        if (hasSchedulerType)
        {
            await AwaitFiberTaskAsync(schedulerType, stopTask).ConfigureAwait(false);
        }
        else
        {
            await stopTask.ConfigureAwait(false);
        }

        if (hasSchedulerType)
        {
            _schedulers[schedulerType].Remove(fiberId);
        }
        else
        {
            foreach (IFiberScheduler scheduler in _schedulers.Values)
            {
                scheduler.Remove(fiberId);
            }
        }

        _fiberSchedulerTypes.TryRemove(fiberId, out _);
        _fibers.TryRemove(fiberId, out _);
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
            fiber.StopInlineAsync().GetAwaiter().GetResult();
        }

        foreach (IFiberScheduler scheduler in _schedulers.Values)
        {
            scheduler.Dispose();
        }

        _fibers.Clear();
        _fiberSchedulerTypes.Clear();
        _schedulers.Clear();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FiberManager));
        }
    }

    internal Fiber? GetInternal(int fiberId)
    {
        _fibers.TryGetValue(fiberId, out Fiber? fiber);
        return fiber;
    }

    internal bool IsDisposedInternal => Volatile.Read(ref _disposed);

    private async Task AwaitFiberTaskAsync(FiberSchedulerType schedulerType, Task task)
    {
        if (schedulerType == FiberSchedulerType.Main && _schedulers[FiberSchedulerType.Main] is MainFiberScheduler scheduler)
        {
            while (!task.IsCompleted)
            {
                scheduler.Update(Environment.TickCount64);
                await Task.Yield();
            }
        }

        await task.ConfigureAwait(false);
    }
}

internal sealed class MainFiberScheduler : IFiberScheduler
{
    private readonly FiberManager _fiberManager;
    private readonly ConcurrentDictionary<int, byte> _fiberIds = new();

    public MainFiberScheduler(FiberManager fiberManager)
    {
        _fiberManager = fiberManager;
    }

    public void Add(int fiberId)
    {
        _fiberIds[fiberId] = 0;
    }

    public void Remove(int fiberId)
    {
        _fiberIds.TryRemove(fiberId, out _);
    }

    public void Update(long timeNowMs)
    {
        foreach (int fiberId in _fiberIds.Keys)
        {
            Fiber? fiber = _fiberManager.GetInternal(fiberId);
            if (fiber == null || fiber.IsStopped)
            {
                _fiberIds.TryRemove(fiberId, out _);
                continue;
            }

            if (fiber.NextWakeTimeMs <= timeNowMs)
            {
                fiber.RunOnce(timeNowMs);
            }
        }
    }

    public void Dispose()
    {
        _fiberIds.Clear();
    }
}

internal sealed class ThreadFiberScheduler : IFiberScheduler
{
    private readonly ConcurrentDictionary<int, ThreadFiberRunner> _runners = new();
    private readonly FiberManager _fiberManager;

    public ThreadFiberScheduler(FiberManager fiberManager)
    {
        _fiberManager = fiberManager;
    }

    public void Add(int fiberId)
    {
        var runner = new ThreadFiberRunner(_fiberManager, fiberId);
        if (_runners.TryAdd(fiberId, runner))
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
        private readonly FiberManager _fiberManager;
        private readonly int _fiberId;
        private readonly CancellationTokenSource _shutdown = new();
        private readonly Thread _thread;

        public ThreadFiberRunner(FiberManager fiberManager, int fiberId)
        {
            _fiberManager = fiberManager;
            _fiberId = fiberId;
            Fiber? fiber = _fiberManager.GetInternal(fiberId);
            _thread = new Thread(Run)
            {
                IsBackground = true,
                Name = fiber == null ? $"Fiber-{fiberId}" : $"Fiber-{fiberId}-{fiber.Name}",
            };
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Dispose()
        {
            _shutdown.Cancel();
            _thread.Join();
            _shutdown.Dispose();
        }

        private void Run()
        {
            while (!_shutdown.IsCancellationRequested && !_fiberManager.IsDisposedInternal)
            {
                Fiber? fiber = _fiberManager.GetInternal(_fiberId);
                if (fiber == null || fiber.IsStopped)
                {
                    return;
                }

                long now = Environment.TickCount64;
                if (fiber.NextWakeTimeMs <= now)
                {
                    fiber.RunOnce(now);
                    now = Environment.TickCount64;
                }

                int sleepMs = FiberSchedulerSleep.GetSleepMs(now, fiber.NextWakeTimeMs);
                Thread.Sleep(sleepMs);
            }
        }
    }
}

internal sealed class ThreadPoolFiberScheduler : IFiberScheduler
{
    private readonly FiberManager _fiberManager;
    private readonly ConcurrentDictionary<int, byte> _fiberIds = new();
    private readonly ConcurrentDictionary<int, long> _wakeTimes = new();
    private readonly ConcurrentQueue<int> _ready = new();
    private readonly PriorityQueue<int, long> _delayQueue = new();
    private readonly object _delayLock = new();
    private readonly AutoResetEvent _wakeSignal = new(false);
    private readonly CancellationTokenSource _shutdown = new();
    private readonly List<Thread> _workers = new();

    public ThreadPoolFiberScheduler(FiberManager fiberManager, int workerCount)
    {
        _fiberManager = fiberManager;
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

    public void Add(int fiberId)
    {
        _fiberIds[fiberId] = 0;
        _ready.Enqueue(fiberId);
        _wakeSignal.Set();
    }

    public void Remove(int fiberId)
    {
        _fiberIds.TryRemove(fiberId, out _);
        _wakeTimes.TryRemove(fiberId, out _);
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
        _fiberIds.Clear();
        _wakeTimes.Clear();
    }

    private void Run()
    {
        while (!_shutdown.IsCancellationRequested && !_fiberManager.IsDisposedInternal)
        {
            long now = Environment.TickCount64;
            MoveDueFibers(now);

            if (_ready.TryDequeue(out int fiberId))
            {
                Fiber? fiber = _fiberManager.GetInternal(fiberId);
                if (fiber != null && !fiber.IsStopped && _fiberIds.ContainsKey(fiberId))
                {
                    try
                    {
                        if (fiber.RunOnce(now))
                        {
                            ScheduleDelay(fiberId, fiber);
                        }
                    }
                    catch (Exception e)
                    {
                        global::GameServer.Core.Log.Error("Fiber", e, $"event=fiber_run_failed fiberId={fiberId}");
                    }
                }

                continue;
            }

            _wakeSignal.WaitOne(FiberSchedulerSleep.GetSleepMs(now, GetNextWakeTime()));
        }
    }

    private void ScheduleDelay(int fiberId, Fiber fiber)
    {
        if (fiber.IsStopped || !_fiberIds.ContainsKey(fiberId))
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
            _wakeTimes[fiberId] = wakeTime;
            _delayQueue.Enqueue(fiberId, wakeTime);
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
                if (!_fiberIds.ContainsKey(fiberId))
                {
                    continue;
                }

                if (!_wakeTimes.TryGetValue(fiberId, out long currentWakeTime) || currentWakeTime != wakeTime)
                {
                    continue;
                }

                _wakeTimes.TryRemove(fiberId, out _);
                _ready.Enqueue(fiberId);
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

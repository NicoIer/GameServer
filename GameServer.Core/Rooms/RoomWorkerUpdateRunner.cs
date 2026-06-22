using System.Diagnostics;

namespace GameServer.Core.Rooms;

public readonly record struct RoomWorkerUpdateRunnerMetrics(
    long TickCount,
    long ExceptionCount,
    TimeSpan LastTickElapsed,
    TimeSpan MaxTickElapsed,
    int RoomCount,
    int ClosingRoomCount);

public sealed class RoomWorkerUpdateRunner : IAsyncDisposable
{
    private readonly IRoomWorker _worker;
    private readonly TimeSpan _networkTickInterval;
    private readonly CancellationTokenSource _shutdown = new();
    private Task? _runTask;
    private bool _stopped;
    private long _tickCount;
    private long _exceptionCount;
    private long _lastTickElapsedTicks;
    private long _maxTickElapsedTicks;
    private int _roomCount;
    private int _closingRoomCount;

    public RoomWorkerUpdateRunner(IRoomWorker worker, int networkTickSleepMs)
    {
        _worker = worker;
        _networkTickInterval = TimeSpan.FromMilliseconds(Math.Max(1, networkTickSleepMs));
    }

    public Task StartAsync()
    {
        _runTask = Task.Run(RunAsync);
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        if (_stopped)
        {
            return;
        }

        _stopped = true;
        _shutdown.Cancel();

        if (_runTask != null)
        {
            await _runTask;
        }

        _worker.Stop();
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _shutdown.Dispose();
    }

    public RoomWorkerUpdateRunnerMetrics GetMetrics()
    {
        return new RoomWorkerUpdateRunnerMetrics(
            Interlocked.Read(ref _tickCount),
            Interlocked.Read(ref _exceptionCount),
            TimeSpan.FromTicks(Interlocked.Read(ref _lastTickElapsedTicks)),
            TimeSpan.FromTicks(Interlocked.Read(ref _maxTickElapsedTicks)),
            Volatile.Read(ref _roomCount),
            Volatile.Read(ref _closingRoomCount));
    }

    private async Task RunAsync()
    {
        using var timer = new PeriodicTimer(_networkTickInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(_shutdown.Token))
            {
                RunTick();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void RunTick()
    {
        long startTimestamp = Stopwatch.GetTimestamp();
        try
        {
            _worker.Update(Environment.TickCount64);
        }
        catch (Exception e)
        {
            Interlocked.Increment(ref _exceptionCount);
            Console.WriteLine(e);
        }
        finally
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            Interlocked.Increment(ref _tickCount);
            Interlocked.Exchange(ref _lastTickElapsedTicks, elapsed.Ticks);
            UpdateMaxTickElapsed(elapsed.Ticks);
            Volatile.Write(ref _roomCount, _worker.RoomCount);
            Volatile.Write(ref _closingRoomCount, _worker.ClosingRoomCount);
        }
    }

    private void UpdateMaxTickElapsed(long elapsedTicks)
    {
        while (true)
        {
            long current = Interlocked.Read(ref _maxTickElapsedTicks);
            if (elapsedTicks <= current)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _maxTickElapsedTicks, elapsedTicks, current) == current)
            {
                return;
            }
        }
    }
}

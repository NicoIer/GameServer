using System.Diagnostics;

namespace GameServer.Core.Rooms;

public readonly record struct RoomWorkerUpdateRunnerMetrics(
    long TickCount,
    long ExceptionCount,
    TimeSpan LastTickElapsed,
    TimeSpan MaxTickElapsed,
    int RoomCount,
    int ClosingRoomCount,
    RoomWorkerMetrics WorkerMetrics);

public sealed class RoomWorkerUpdateRunner : IAsyncDisposable
{
    private const long MetricsLogIntervalMs = 10_000;

    private readonly IRoomWorker _worker;
    private readonly TimeSpan _networkTickInterval;
    private readonly CancellationTokenSource _shutdown = new();
    private Task? _runTask;
    private bool _stopped;
    private long _tickCount;
    private long _exceptionCount;
    private long _lastTickElapsedTicks;
    private long _maxTickElapsedTicks;
    private long _nextMetricsLogTimeMs;
    private int _roomCount;
    private int _closingRoomCount;

    public RoomWorkerUpdateRunner(IRoomWorker worker, int networkTickSleepMs)
    {
        _worker = worker;
        _networkTickInterval = TimeSpan.FromMilliseconds(Math.Max(1, networkTickSleepMs));
        _nextMetricsLogTimeMs = Environment.TickCount64 + MetricsLogIntervalMs;
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
            Volatile.Read(ref _closingRoomCount),
            _worker.GetMetrics());
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
        long timeNowMs = Environment.TickCount64;
        long startTimestamp = Stopwatch.GetTimestamp();
        try
        {
            _worker.Update(timeNowMs);
        }
        catch (Exception e)
        {
            Interlocked.Increment(ref _exceptionCount);
            global::GameServer.Core.Log.Error("Room", e, "event=room_worker_tick_failed");
        }
        finally
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimestamp);
            Interlocked.Increment(ref _tickCount);
            Interlocked.Exchange(ref _lastTickElapsedTicks, elapsed.Ticks);
            UpdateMaxTickElapsed(elapsed.Ticks);
            Volatile.Write(ref _roomCount, _worker.RoomCount);
            Volatile.Write(ref _closingRoomCount, _worker.ClosingRoomCount);
            LogMetrics(timeNowMs);
        }
    }

    private void LogMetrics(long timeNowMs)
    {
        if (timeNowMs < _nextMetricsLogTimeMs)
        {
            return;
        }

        _nextMetricsLogTimeMs = timeNowMs + MetricsLogIntervalMs;
        RoomWorkerUpdateRunnerMetrics metrics = GetMetrics();
        RoomWorkerMetrics workerMetrics = metrics.WorkerMetrics;
        global::GameServer.Core.Log.Info(
            "Room",
            $"event=room_worker_metrics tickCount={metrics.TickCount} exceptionCount={metrics.ExceptionCount} " +
            $"lastTickMs={metrics.LastTickElapsed.TotalMilliseconds:F3} maxTickMs={metrics.MaxTickElapsed.TotalMilliseconds:F3} " +
            $"roomCount={workerMetrics.RoomCount} closingRoomCount={workerMetrics.ClosingRoomCount} " +
            $"onlineConnectionCount={workerMetrics.OnlineConnectionCount} requestCount={workerMetrics.RequestCount} " +
            $"requestErrorCount={workerMetrics.RequestErrorCount} lastRequestMs={workerMetrics.LastRequestElapsed.TotalMilliseconds:F3} " +
            $"maxRequestMs={workerMetrics.MaxRequestElapsed.TotalMilliseconds:F3} roomCreatedCount={workerMetrics.RoomCreatedCount} " +
            $"roomClosedCount={workerMetrics.RoomClosedCount} disconnectionCount={workerMetrics.DisconnectionCount} " +
            $"roomConnectCount={workerMetrics.RoomConnectCount} pushSentCount={workerMetrics.PushSentCount} " +
            $"pushDroppedCount={workerMetrics.PushDroppedCount}");
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

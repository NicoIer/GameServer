namespace GameServer.Core.Rooms;

public sealed class RoomWorkerUpdateRunner : IAsyncDisposable
{
    private readonly IRoomWorker _worker;
    private readonly int _networkTickSleepMs;
    private readonly CancellationTokenSource _shutdown = new();
    private Task? _runTask;
    private bool _stopped;

    public RoomWorkerUpdateRunner(IRoomWorker worker, int networkTickSleepMs)
    {
        _worker = worker;
        _networkTickSleepMs = networkTickSleepMs;
    }

    public Task StartAsync()
    {
        _runTask = Task.Run(Run);
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

    private void Run()
    {
        while (!_shutdown.IsCancellationRequested)
        {
            Thread.Sleep(_networkTickSleepMs);
            if (_shutdown.IsCancellationRequested)
            {
                break;
            }

            try
            {
                _worker.Update(Environment.TickCount64);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

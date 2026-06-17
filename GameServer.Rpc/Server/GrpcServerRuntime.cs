namespace GameServer.Rpc.Server;

public sealed class GrpcServerRuntime : IAsyncDisposable
{
    private readonly CancellationTokenSource _dispatchCts = new();
    private readonly GrpcServerHost _host;
    private Task? _dispatchTask;
    private bool _stopped;

    public GrpcServerRuntime(int port)
    {
        Dispatcher = new GrpcMessageDispatcher();
        _host = new GrpcServerHost(port, Dispatcher);
    }

    public GrpcMessageDispatcher Dispatcher { get; }

    public void MapGrpcService<TService>() where TService : class
    {
        _host.MapGrpcService<TService>();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _host.StartAsync(cancellationToken);
        _dispatchTask = Task.Run(RunDispatchLoopAsync, CancellationToken.None);
    }

    public async Task StopAsync()
    {
        if (_stopped)
        {
            return;
        }

        _stopped = true;
        _dispatchCts.Cancel();

        if (_dispatchTask != null)
        {
            try
            {
                await _dispatchTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        using var stopCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await _host.StopAsync(stopCts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _dispatchCts.Dispose();
    }

    private async Task RunDispatchLoopAsync()
    {
        while (!_dispatchCts.IsCancellationRequested)
        {
            Dispatcher.ProcessMessages();
            await Task.Delay(1, _dispatchCts.Token);
        }
    }
}

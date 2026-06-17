using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Core.Grpc;

public sealed class GrpcServerRuntime : IAsyncDisposable
{
    private readonly GrpcServerHost _host;
    private bool _stopped;

    public GrpcServerRuntime(int port, Action<IServiceCollection> configureServices)
    {
        _host = new GrpcServerHost(port, configureServices);
    }

    public void MapGrpcService<TService>() where TService : class
    {
        _host.MapGrpcService<TService>();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return _host.StartAsync(cancellationToken);
    }

    public async Task StopAsync()
    {
        if (_stopped)
        {
            return;
        }

        _stopped = true;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await _host.StopAsync(cts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}

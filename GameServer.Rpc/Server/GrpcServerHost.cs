using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameServer.Rpc.Server;

public sealed class GrpcServerHost
{
    private readonly WebApplication _app;

    public GrpcServerHost(int port, GrpcMessageDispatcher dispatcher)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(GrpcServerHost).Assembly.GetName().Name,
        });

        builder.Services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(5));
        builder.Services.AddSingleton<IHostLifetime, NullLifetime>();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(port, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });

            options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(30);
            options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(60);
            options.Limits.Http2.MaxStreamsPerConnection = 100;
            options.Limits.Http2.InitialConnectionWindowSize = 1024 * 1024;
            options.Limits.Http2.InitialStreamWindowSize = 1024 * 1024;

            options.Limits.MaxConcurrentConnections = 1000;
            options.Limits.MaxConcurrentUpgradedConnections = 1000;
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
            options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
        });

        builder.Services.AddSingleton(dispatcher);
        builder.Services.AddGrpc(options =>
        {
            options.MaxReceiveMessageSize = 10 * 1024 * 1024;
            options.MaxSendMessageSize = 10 * 1024 * 1024;
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        _app = builder.Build();
    }

    public void MapGrpcService<TService>() where TService : class
    {
        _app.MapGrpcService<TService>();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return _app.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return _app.StopAsync(cancellationToken);
    }

    private sealed class NullLifetime : IHostLifetime
    {
        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

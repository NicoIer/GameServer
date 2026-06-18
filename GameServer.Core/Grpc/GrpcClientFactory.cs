using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GameServer.Core.Grpc;

public static class GrpcClientFactory
{
    private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(logging =>
    {
        logging.ClearProviders();
        logging.AddSerilog(GameServer.Core.Log.SerilogLogger, dispose: false);
    });

    public static GrpcChannel CreateChannel(string address)
    {
        var handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        };

        return GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpHandler = handler,
            LoggerFactory = LoggerFactory,
        });
    }
}

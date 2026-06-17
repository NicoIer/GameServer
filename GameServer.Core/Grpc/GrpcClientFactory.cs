using Grpc.Net.Client;

namespace GameServer.Core.Grpc;

public static class GrpcClientFactory
{
    public static GrpcChannel CreateChannel(string address)
    {
        var handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        };

        return GrpcChannel.ForAddress(address, new GrpcChannelOptions { HttpHandler = handler });
    }
}

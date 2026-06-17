using Grpc.Core;
using Grpc.Net.Client;

namespace GameServer.Rpc.Clients;

public sealed class GrpcTestRpcClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly GrpcTest.GrpcTestClient _client;

    public GrpcTestRpcClient(string host, int port)
    {
        var handler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        };

        _channel = GrpcChannel.ForAddress(
            $"http://{host}:{port}",
            new GrpcChannelOptions { HttpHandler = handler });
        _client = new GrpcTest.GrpcTestClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
    }

    public async Task<GrpcTestReply> PingAsync(string message, int timeoutMs = 5000)
    {
        var request = new GrpcTestRequest { Message = message };
        var options = new CallOptions(deadline: DateTime.UtcNow.AddMilliseconds(timeoutMs));
        return await _client.PingAsync(request, options);
    }
}

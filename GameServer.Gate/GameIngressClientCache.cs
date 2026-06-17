using GameServer.Core.Grpc;
using GameServer.Core.Protocol;
using Grpc.Net.Client;

namespace GameServer.Gate;

public sealed class GameIngressClientCache : IDisposable
{
    private readonly Dictionary<string, GrpcChannel> _channels = new();
    private readonly Dictionary<string, GameIngress.GameIngressClient> _clients = new();

    public GameIngress.GameIngressClient GetClient(string address)
    {
        if (_clients.TryGetValue(address, out GameIngress.GameIngressClient? client))
        {
            return client;
        }

        GrpcChannel channel = GrpcClientFactory.CreateChannel(address);
        client = new GameIngress.GameIngressClient(channel);

        _channels[address] = channel;
        _clients[address] = client;

        return client;
    }

    public void Dispose()
    {
        foreach (GrpcChannel channel in _channels.Values)
        {
            channel.Dispose();
        }
    }
}

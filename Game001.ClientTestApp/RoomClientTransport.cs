using System.Net.Sockets;
using GameServer.Core.Network;
using GameServer.Core.Protocol;

namespace Game001.ClientTestApp;

public interface IRoomClientTransport : IAsyncDisposable
{
    DirectTransportProtocol Protocol { get; }

    Task WriteAsync<T>(T message)
        where T : struct;

    Task<T?> ReadAsync<T>()
        where T : struct;
}

public static class RoomClientTransportFactory
{
    public static Task<IRoomClientTransport> ConnectAsync(PrepareRoomConnectionReply connection)
    {
        if (connection.DirectProtocol == DirectTransportProtocol.Tcp)
        {
            return TcpRoomClientTransport.ConnectAsync(connection.Host, connection.Port);
        }

        throw new NotSupportedException($"unsupported room transport protocol={connection.DirectProtocol}");
    }
}

public sealed class TcpRoomClientTransport : IRoomClientTransport
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;

    private TcpRoomClientTransport(TcpClient client)
    {
        _client = client;
        _stream = client.GetStream();
    }

    public DirectTransportProtocol Protocol => DirectTransportProtocol.Tcp;

    public static async Task<IRoomClientTransport> ConnectAsync(string host, int port)
    {
        var client = new TcpClient();
        await client.ConnectAsync(host, port);
        return new TcpRoomClientTransport(client);
    }

    public Task WriteAsync<T>(T message)
        where T : struct
    {
        return GameTcpFrame.WriteAsync(_stream, message);
    }

    public Task<T?> ReadAsync<T>()
        where T : struct
    {
        return GameTcpFrame.ReadAsync<T>(_stream);
    }

    public ValueTask DisposeAsync()
    {
        _stream.Dispose();
        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}

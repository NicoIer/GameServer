using System.Net;
using System.Net.Sockets;
using GameServer.Core.Network;
using GameServer.Core.Protocol;

namespace Game001.Room;

public sealed class Game001RoomTcpServer : IGameRoomTransportServer
{
    private readonly CenterService.CenterServiceClient _centerClient;
    private readonly Game001RoomPacketHandler _handler;
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _shutdown = new();
    private Task? _acceptTask;
    private bool _stopped;

    public Game001RoomTcpServer(int port, CenterService.CenterServiceClient centerClient, Game001RoomPacketHandler handler)
    {
        _centerClient = centerClient;
        _handler = handler;
        _listener = new TcpListener(IPAddress.Loopback, port);
        Address = $"127.0.0.1:{port}";
    }

    public DirectTransportProtocol Protocol => DirectTransportProtocol.Tcp;
    public string Address { get; }

    public Task StartAsync()
    {
        _listener.Start();
        _acceptTask = Task.Run(AcceptLoopAsync);
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
        _listener.Stop();

        if (_acceptTask != null)
        {
            try
            {
                await _acceptTask;
            }
            catch (OperationCanceledException)
            {
            }
            catch (SocketException)
            {
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _shutdown.Dispose();
    }

    private async Task AcceptLoopAsync()
    {
        while (!_shutdown.IsCancellationRequested)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync(_shutdown.Token);
            _ = Task.Run(() => HandleConnectionAsync(client, _shutdown.Token));
        }
    }

    private async Task HandleConnectionAsync(TcpClient client, CancellationToken cancellationToken)
    {
        using (client)
        {
            NetworkStream stream = client.GetStream();
            try
            {
                GamePacket? firstPacket = await GameTcpFrame.ReadAsync(stream, cancellationToken);
                if (firstPacket == null)
                {
                    return;
                }

                if (firstPacket.Value.MessageId != GameMessageIds.Game001RoomConnectRequest)
                {
                    await WriteConnectionReplyAsync(stream, ErrorCode.InvalidRequest, 0, string.Empty, "first packet must be RoomConnectRequest", cancellationToken);
                    return;
                }

                RoomConnectRequest connectRequest = GamePacketSerializer.Unpack<RoomConnectRequest>(firstPacket.Value);
                ValidateTokenReply validateReply = await _centerClient.ValidateTokenAsync(new ValidateTokenRequest
                {
                    Token = connectRequest.ConnectTicket,
                }, cancellationToken: cancellationToken);

                if (validateReply.Error != ErrorCode.Success)
                {
                    await WriteConnectionReplyAsync(stream, ErrorCode.Unauthorized, 0, connectRequest.RoomId, "unauthorized", cancellationToken);
                    return;
                }

                await WriteConnectionReplyAsync(stream, ErrorCode.Success, validateReply.Uid, connectRequest.RoomId, "connected", cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    GamePacket? packet = await GameTcpFrame.ReadAsync(stream, cancellationToken);
                    if (packet == null)
                    {
                        return;
                    }

                    GameResponse response = _handler.HandlePacket(validateReply.Uid, connectRequest.RoomId, packet.Value);
                    await GameTcpFrame.WriteRawAsync(stream, response.Data.ToByteArray(), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (IOException)
            {
            }
            catch (SocketException)
            {
            }
        }
    }

    private static Task WriteConnectionReplyAsync(
        NetworkStream stream,
        int error,
        long uid,
        string roomId,
        string message,
        CancellationToken cancellationToken)
    {
        RoomConnectionReply reply = new RoomConnectionReply
        {
            Error = error,
            Uid = uid,
            RoomId = roomId,
            Message = message,
        };

        GamePacket packet = GamePacketSerializer.Pack(GameMessageIds.Game001RoomConnectionReply, reply);
        return GameTcpFrame.WriteAsync(stream, packet, cancellationToken);
    }
}

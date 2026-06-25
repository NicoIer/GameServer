using System.Collections.Concurrent;
using GameServer.Core.Network;
using GameServer.Core.Protocol;
using kcp2k;
using Network;
using Network.Server;
using UnityToolkit;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace GameServer.Core.Rooms;

public sealed class UnityRoomTransportServer : IGameRoomTransportServer
{
    private static readonly ushort RoomHandshakeReqHash = TypeId<RoomHandshakeReq>.stableId16;

    private readonly CenterService.CenterServiceClient _centerClient;
    private readonly IRoomWorker _worker;
    private readonly NetworkServer _server;
    private readonly DirectTransportProtocol _protocol;
    private readonly ReqRspServerCenter _connectCenter = new();
    private readonly ConcurrentDictionary<int, int> _workerConnectionIds = new();
    private readonly CancellationTokenSource _shutdown = new();
    private readonly TimeSpan _networkTickInterval;
    private readonly float _networkTickDeltaSeconds;
    private Task? _runTask;
    private bool _stopped;

    public UnityRoomTransportServer(
        int port,
        CenterService.CenterServiceClient centerClient,
        IRoomWorker worker,
        int networkTickSleepMs,
        string? advertisedAddress = null)
        : this(
            DirectTransportProtocol.Tcp,
            new TelepathyServerSocket((ushort)port),
            port,
            centerClient,
            worker,
            networkTickSleepMs,
            advertisedAddress)
    {
    }

    public UnityRoomTransportServer(
        DirectTransportProtocol protocol,
        IServerSocket socket,
        int port,
        CenterService.CenterServiceClient centerClient,
        IRoomWorker worker,
        int networkTickSleepMs,
        string? advertisedAddress = null)
    {
        _centerClient = centerClient;
        _worker = worker;
        _protocol = protocol;
        int networkTickIntervalMs = Math.Max(1, networkTickSleepMs);
        _networkTickInterval = TimeSpan.FromMilliseconds(networkTickIntervalMs);
        _networkTickDeltaSeconds = networkTickIntervalMs / 1000f;
        _server = new NetworkServer(socket, 1000, false);
        _server.AddMsgHandler<ReqHead>(OnReqHead);
        _server.socket.OnDisconnected += OnDisconnected;

        _connectCenter.Register<RoomHandshakeReq, RoomHandshakeRsp>(HandshakeAsync);

        Address = string.IsNullOrWhiteSpace(advertisedAddress) ? $"127.0.0.1:{port}" : advertisedAddress;
    }

    public DirectTransportProtocol Protocol => _protocol;
    public string Address { get; }

    public static UnityRoomTransportServer CreateTcp(
        int port,
        CenterService.CenterServiceClient centerClient,
        IRoomWorker worker,
        int networkTickSleepMs,
        string? advertisedAddress = null)
    {
        return new UnityRoomTransportServer(
            DirectTransportProtocol.Tcp,
            new TelepathyServerSocket((ushort)port),
            port,
            centerClient,
            worker,
            networkTickSleepMs,
            advertisedAddress);
    }

    public static UnityRoomTransportServer CreateKcp(
        int port,
        CenterService.CenterServiceClient centerClient,
        IRoomWorker worker,
        int networkTickSleepMs,
        string? advertisedAddress = null)
    {
        return new UnityRoomTransportServer(
            DirectTransportProtocol.Kcp,
            new KcpServerSocket(new KcpConfig(), (ushort)port, KcpChannel.Reliable),
            port,
            centerClient,
            worker,
            networkTickSleepMs,
            advertisedAddress);
    }

    public Task StartAsync()
    {
        _server.Run(false);
        _runTask = Task.Run(RunAsync);
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
        _server.Stop();

        if (_runTask != null)
        {
            await _runTask;
        }

        foreach (int workerConnectionId in _workerConnectionIds.Values)
        {
            await _worker.RemoveConnectionAsync(workerConnectionId);
        }

        _workerConnectionIds.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _server.Dispose();
        _shutdown.Dispose();
    }

    private async Task RunAsync()
    {
        using var timer = new PeriodicTimer(_networkTickInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(_shutdown.Token))
            {
                _server.OnUpdate(_networkTickDeltaSeconds);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void OnReqHead(in int connectionId, in ReqHead request)
    {
        _ = HandleReqHeadAsync(connectionId, request);
    }

    private async Task HandleReqHeadAsync(int connectionId, ReqHead request)
    {
        NetworkBuffer responsePayloadWriter = NetworkBufferPool.Shared.Get();
        RspHead response;
        if (!_workerConnectionIds.TryGetValue(connectionId, out int workerConnectionId))
        {
            if (request.reqHash != RoomHandshakeReqHash)
            {
                response = new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, "room connection is not authenticated", default);
            }
            else
            {
                response = await _connectCenter.HandleRequestAsync(connectionId, request, responsePayloadWriter);
            }
        }
        else
        {
            response = await _worker.HandleRequestAsync(workerConnectionId, request, responsePayloadWriter);
        }

        _server.Send(connectionId, response);
        NetworkBufferPool.Shared.Return(responsePayloadWriter);
    }

    private void OnDisconnected(int connectionId)
    {
        if (_workerConnectionIds.TryRemove(connectionId, out int workerConnectionId))
        {
            _ = _worker.RemoveConnectionAsync(workerConnectionId);
        }
    }

    internal async ValueTask<(RoomHandshakeRsp rsp, NetworkErrorCode errorCode, string errorMsg)> HandshakeAsync(int connectionId, RoomHandshakeReq req)
    {
        if (_workerConnectionIds.ContainsKey(connectionId))
        {
            return (new RoomHandshakeRsp(), NetworkErrorCode.InvalidArgument, "room connection already authenticated");
        }

        ValidateTokenReply validateReply = await _centerClient.ValidateTokenAsync(new ValidateTokenRequest
        {
            Token = req.ConnectTicket,
        });

        if (validateReply.Error != ProtocolErrorCode.Success)
        {
            return (new RoomHandshakeRsp(), NetworkErrorCode.InvalidArgument, "unauthorized");
        }

        int workerConnectionId = await _worker.AddConnectionAsync(validateReply.Uid, string.Empty);
        if (!_workerConnectionIds.TryAdd(connectionId, workerConnectionId))
        {
            await _worker.RemoveConnectionAsync(workerConnectionId);
            return (new RoomHandshakeRsp { Uid = validateReply.Uid }, NetworkErrorCode.InvalidArgument, "room connection already authenticated");
        }

        _worker.PushHub.Register(workerConnectionId, push => _server.Send(connectionId, push));

        var connected = new RoomHandshakeRsp
        {
            Uid = validateReply.Uid,
        };
        return (connected, NetworkErrorCode.Success, "handshake ok");
    }
}

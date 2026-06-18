using System.Collections.Concurrent;
using GameServer.Core.Generated;
using GameServer.Core.Network;
using GameServer.Core.Protocol;
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
    private readonly ReqRspServerCenter _connectCenter = new();
    private readonly ConcurrentDictionary<int, int> _workerConnectionIds = new();
    private readonly CancellationTokenSource _shutdown = new();
    private readonly int _networkTickSleepMs;
    private Task? _runTask;
    private bool _stopped;

    public UnityRoomTransportServer(int port, CenterService.CenterServiceClient centerClient, IRoomWorker worker, int networkTickSleepMs)
    {
        _centerClient = centerClient;
        _worker = worker;
        _networkTickSleepMs = networkTickSleepMs;
        _server = new NetworkServer(new TelepathyServerSocket((ushort)port), 1000, false);
        _server.AddMsgHandler<ReqHead>(OnReqHead);
        _server.socket.OnDisconnected += OnDisconnected;

        var handlers = new RoomHandshakeReqRspHandlers(this);
        NetworkReqRspInitializer.RegisterAll(_connectCenter, handlers);

        Address = $"127.0.0.1:{port}";
    }

    public DirectTransportProtocol Protocol => DirectTransportProtocol.Tcp;
    public string Address { get; }

    public Task StartAsync()
    {
        _server.Run(false);
        _runTask = Task.Run(Run);
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

    private void Run()
    {
        while (!_shutdown.IsCancellationRequested)
        {
            Thread.Sleep(_networkTickSleepMs);
            if (_shutdown.IsCancellationRequested)
            {
                break;
            }

            _server.OnUpdate(_networkTickSleepMs / 1000f);
        }
    }

    private void OnReqHead(in int connectionId, in ReqHead request)
    {
        _ = HandleReqHeadAsync(connectionId, request);
    }

    private async Task HandleReqHeadAsync(int connectionId, ReqHead request)
    {
        RspHead response;
        if (!_workerConnectionIds.TryGetValue(connectionId, out int workerConnectionId))
        {
            if (request.reqHash != RoomHandshakeReqHash)
            {
                response = new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, "room connection is not authenticated", default);
            }
            else
            {
                response = await _connectCenter.HandleRequestAsync(connectionId, request);
            }
        }
        else
        {
            response = await _worker.HandleRequestAsync(workerConnectionId, request);
        }

        _server.Send(connectionId, response, true);
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
            var alreadyConnected = new RoomHandshakeRsp
            {
                Error = ProtocolErrorCode.InvalidRequest,
                Message = "room connection already authenticated",
            };
            return (alreadyConnected, NetworkErrorCode.Success, string.Empty);
        }

        ValidateTokenReply validateReply = await _centerClient.ValidateTokenAsync(new ValidateTokenRequest
        {
            Token = req.ConnectTicket,
        });

        if (validateReply.Error != ProtocolErrorCode.Success)
        {
            var unauthorized = new RoomHandshakeRsp
            {
                Error = ProtocolErrorCode.Unauthorized,
                Message = "unauthorized",
            };
            return (unauthorized, NetworkErrorCode.Success, string.Empty);
        }

        int workerConnectionId = await _worker.AddConnectionAsync(validateReply.Uid, string.Empty);
        if (!_workerConnectionIds.TryAdd(connectionId, workerConnectionId))
        {
            await _worker.RemoveConnectionAsync(workerConnectionId);
            var alreadyConnected = new RoomHandshakeRsp
            {
                Error = ProtocolErrorCode.InvalidRequest,
                Uid = validateReply.Uid,
                Message = "room connection already authenticated",
            };
            return (alreadyConnected, NetworkErrorCode.Success, string.Empty);
        }

        var connected = new RoomHandshakeRsp
        {
            Error = ProtocolErrorCode.Success,
            Uid = validateReply.Uid,
            Message = "handshake ok",
        };
        return (connected, NetworkErrorCode.Success, string.Empty);
    }

}

public sealed partial class RoomHandshakeReqRspHandlers
{
    private readonly UnityRoomTransportServer _server;

    public RoomHandshakeReqRspHandlers(UnityRoomTransportServer server)
    {
        _server = server;
    }

    public static partial class RoomHandshakeReqRsp
    {
        public static ValueTask<(RoomHandshakeRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(
            RoomHandshakeReqRspHandlers self,
            int connectionId,
            RoomHandshakeReq req)
        {
            return self._server.HandshakeAsync(connectionId, req);
        }
    }
}

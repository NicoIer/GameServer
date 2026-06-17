using Game001.Core;
using GameServer.Core.Fibers;
using GameServer.Core.Protocol;
using Google.Protobuf;
using MemoryPack;
using Network;
using UnityToolkit;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed class Game001RoomWorker : IDisposable
{
    private const string DefaultRoomId = "room-001";

    private static readonly ushort CreateRoomReqHash = TypeId<CreateRoomReq>.stableId16;
    private static readonly ushort JoinRoomReqHash = TypeId<JoinRoomReq>.stableId16;
    private static readonly ushort LeaveRoomReqHash = TypeId<LeaveRoomReq>.stableId16;
    private static readonly ushort RoomPingReqHash = TypeId<RoomPingReq>.stableId16;
    private static readonly ushort CreateRoomRspHash = TypeId<CreateRoomRsp>.stableId16;
    private static readonly ushort JoinRoomRspHash = TypeId<JoinRoomRsp>.stableId16;
    private static readonly ushort LeaveRoomRspHash = TypeId<LeaveRoomRsp>.stableId16;
    private static readonly ushort RoomPingRspHash = TypeId<RoomPingRsp>.stableId16;

    private readonly FiberManager _fiberManager = new();
    private readonly Game001RoomConnectionRegistry _connections;
    private readonly Dictionary<string, RoomRuntimeHandle> _rooms = new(StringComparer.Ordinal);
    private readonly SemaphoreSlim _roomLock = new(1, 1);
    private readonly int _roomFrameRate;
    private bool _stopped;

    public Game001RoomWorker(Game001RoomConnectionRegistry connections, int roomFrameRate)
    {
        _connections = connections;
        _roomFrameRate = roomFrameRate;
    }

    public Task<int> AddConnectionAsync(long uid, string roomId)
    {
        if (_stopped)
        {
            return Task.FromCanceled<int>(new CancellationToken(true));
        }

        return Task.FromResult(_connections.Add(uid, roomId));
    }

    public Task RemoveConnectionAsync(int connectionId)
    {
        _connections.Remove(connectionId);
        return Task.CompletedTask;
    }

    public async Task<RspHead> HandleRequestAsync(int connectionId, ReqHead request)
    {
        if (_stopped)
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InternalError, "room worker stopped", default);
        }

        if (!_connections.TryGet(connectionId, out Game001RoomConnectionContext context))
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, $"missing room connection context connectionId={connectionId}", default);
        }

        string roomId;
        try
        {
            if (!TryResolveRouteRoomId(request, context.RoomId, out roomId))
            {
                return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.NotSupported, "room request is not registered", default);
            }
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.InvalidArgument, "invalid room request payload", default);
        }

        RoomRuntimeHandle? room = await ResolveRoomAsync(request, roomId);
        if (room == null)
        {
            return CreateRoomNotFoundResponse(request, roomId);
        }

        RspHead response = await room.Fiber.CallAsync(() => room.Module.HandleRequest(connectionId, request));
        if (response.error == NetworkErrorCode.Success && IsConnectionRoomBindingRequest(request.reqHash))
        {
            _connections.TrySetRoom(connectionId, roomId);
        }
        else if (response.error == NetworkErrorCode.Success && request.reqHash == LeaveRoomReqHash)
        {
            _connections.TrySetRoom(connectionId, string.Empty);
        }

        return response;
    }

    public async Task<GameResponse> HandleDataAsync(long uid, ByteString data)
    {
        int connectionId = _connections.Add(uid, string.Empty);
        try
        {
            ReqHead request = MemoryPackSerializer.Deserialize<ReqHead>(data.ToByteArray());
            RspHead response = await HandleRequestAsync(connectionId, request);
            return new GameResponse
            {
                Error = ProtocolErrorCode.Success,
                Data = ByteString.CopyFrom(MemoryPackSerializer.Serialize(response)),
            };
        }
        catch
        {
            return new GameResponse { Error = ProtocolErrorCode.InvalidRequest };
        }
        finally
        {
            _connections.Remove(connectionId);
        }
    }

    public void Update(long timeNowMs)
    {
        _fiberManager.UpdateMain(timeNowMs);
    }

    public void Stop()
    {
        _stopped = true;
    }

    public void Dispose()
    {
        Stop();
        _roomLock.Dispose();
        _fiberManager.Dispose();
    }

    private async Task<RoomRuntimeHandle?> ResolveRoomAsync(ReqHead request, string roomId)
    {
        if (request.reqHash == CreateRoomReqHash)
        {
            return await GetOrCreateRoomAsync(roomId);
        }

        await _roomLock.WaitAsync();
        try
        {
            if (_rooms.TryGetValue(roomId, out RoomRuntimeHandle handle))
            {
                return handle;
            }
        }
        finally
        {
            _roomLock.Release();
        }

        return null;
    }

    private async Task<RoomRuntimeHandle> GetOrCreateRoomAsync(string roomId)
    {
        await _roomLock.WaitAsync();
        try
        {
            if (_rooms.TryGetValue(roomId, out RoomRuntimeHandle existing))
            {
                return existing;
            }

            var module = new Game001RoomFiberModule(roomId, _connections, _roomFrameRate);
            Fiber fiber = await _fiberManager.CreateAsync(FiberSchedulerType.ThreadPool, $"Game001.RoomRoot.{roomId}", module);
            var handle = new RoomRuntimeHandle(fiber, module);
            _rooms.Add(roomId, handle);
            return handle;
        }
        finally
        {
            _roomLock.Release();
        }
    }

    private static bool TryResolveRouteRoomId(ReqHead request, string contextRoomId, out string roomId)
    {
        if (request.reqHash == CreateRoomReqHash)
        {
            CreateRoomReq req = MemoryPackSerializer.Deserialize<CreateRoomReq>(request.payload);
            roomId = ResolveRoomId(req.RoomId, contextRoomId);
            return true;
        }

        if (request.reqHash == JoinRoomReqHash)
        {
            JoinRoomReq req = MemoryPackSerializer.Deserialize<JoinRoomReq>(request.payload);
            roomId = ResolveRoomId(req.RoomId, contextRoomId);
            return true;
        }

        if (request.reqHash == LeaveRoomReqHash)
        {
            LeaveRoomReq req = MemoryPackSerializer.Deserialize<LeaveRoomReq>(request.payload);
            roomId = ResolveRoomId(req.RoomId, contextRoomId);
            return true;
        }

        if (request.reqHash == RoomPingReqHash)
        {
            RoomPingReq req = MemoryPackSerializer.Deserialize<RoomPingReq>(request.payload);
            roomId = ResolveRoomId(req.RoomId, contextRoomId);
            return true;
        }

        roomId = string.Empty;
        return false;
    }

    private static string ResolveRoomId(string? messageRoomId, string contextRoomId)
    {
        if (!string.IsNullOrWhiteSpace(messageRoomId))
        {
            return messageRoomId;
        }

        if (!string.IsNullOrWhiteSpace(contextRoomId))
        {
            return contextRoomId;
        }

        return DefaultRoomId;
    }

    private static bool IsConnectionRoomBindingRequest(ushort reqHash)
    {
        return reqHash == CreateRoomReqHash || reqHash == JoinRoomReqHash;
    }

    private static RspHead CreateRoomNotFoundResponse(ReqHead request, string roomId)
    {
        string message = $"room not found room={roomId}";
        if (request.reqHash == JoinRoomReqHash)
        {
            var rsp = new JoinRoomRsp
            {
                Error = ProtocolErrorCode.RoomNotFound,
                Success = false,
                Message = message,
                RoomId = roomId,
                PlayerCount = 0,
                ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            };
            return new RspHead(request.index, request.reqHash, JoinRoomRspHash, NetworkErrorCode.Success, string.Empty, MemoryPackSerializer.Serialize(rsp));
        }

        if (request.reqHash == LeaveRoomReqHash)
        {
            var rsp = new LeaveRoomRsp
            {
                Error = ProtocolErrorCode.RoomNotFound,
                Success = false,
                Message = message,
                RoomId = roomId,
                PlayerCount = 0,
                ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            };
            return new RspHead(request.index, request.reqHash, LeaveRoomRspHash, NetworkErrorCode.Success, string.Empty, MemoryPackSerializer.Serialize(rsp));
        }

        if (request.reqHash == RoomPingReqHash)
        {
            var rsp = new RoomPingRsp
            {
                Error = ProtocolErrorCode.RoomNotFound,
                Success = false,
                Message = message,
                RoomId = roomId,
                PlayerCount = 0,
                ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            };
            return new RspHead(request.index, request.reqHash, RoomPingRspHash, NetworkErrorCode.Success, string.Empty, MemoryPackSerializer.Serialize(rsp));
        }

        return new RspHead(request.index, request.reqHash, 0, NetworkErrorCode.NotSupported, message, default);
    }

    private readonly record struct RoomRuntimeHandle(Fiber Fiber, Game001RoomFiberModule Module);
}

using Game001.Core;
using GameServer.Core.Rooms;
using MemoryPack;
using Network;
using UnityToolkit;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed class Game001RoomWorker : RoomWorkerBase<Game001RoomFiberModule>
{
    private const string DefaultRoomId = "room-001";

    private static readonly ushort CreateRoomReqHash = TypeId<CreateRoomReq>.stableId16;
    private static readonly ushort JoinRoomReqHash = TypeId<JoinRoomReq>.stableId16;
    private static readonly ushort LeaveRoomReqHash = TypeId<LeaveRoomReq>.stableId16;
    private static readonly ushort RoomPingReqHash = TypeId<RoomPingReq>.stableId16;
    private static readonly ushort JoinRoomRspHash = TypeId<JoinRoomRsp>.stableId16;
    private static readonly ushort LeaveRoomRspHash = TypeId<LeaveRoomRsp>.stableId16;
    private static readonly ushort RoomPingRspHash = TypeId<RoomPingRsp>.stableId16;

    public Game001RoomWorker(RoomConnectionRegistry connections, int roomFrameRate)
        : base(connections, roomFrameRate)
    {
    }

    protected override bool TryResolveRoomId(ReqHead request, RoomConnectionContext context, out string roomId)
    {
        if (request.reqHash == CreateRoomReqHash)
        {
            CreateRoomReq req = MemoryPackSerializer.Deserialize<CreateRoomReq>(request.payload);
            roomId = ResolveRoomId(req.RoomId, context.RoomId);
            return true;
        }

        if (request.reqHash == JoinRoomReqHash)
        {
            JoinRoomReq req = MemoryPackSerializer.Deserialize<JoinRoomReq>(request.payload);
            return TryResolveConnectedRoomId(req.RoomId, context.RoomId, out roomId);
        }

        if (request.reqHash == LeaveRoomReqHash)
        {
            LeaveRoomReq req = MemoryPackSerializer.Deserialize<LeaveRoomReq>(request.payload);
            return TryResolveConnectedRoomId(req.RoomId, context.RoomId, out roomId);
        }

        if (request.reqHash == RoomPingReqHash)
        {
            RoomPingReq req = MemoryPackSerializer.Deserialize<RoomPingReq>(request.payload);
            return TryResolveConnectedRoomId(req.RoomId, context.RoomId, out roomId);
        }

        roomId = string.Empty;
        return false;
    }

    protected override bool IsCreateRoomRequest(ReqHead request)
    {
        return request.reqHash == CreateRoomReqHash;
    }

    protected override bool ShouldBindConnectionRoom(ReqHead request, RspHead response)
    {
        return false;
    }

    protected override bool ShouldClearConnectionRoom(ReqHead request, RspHead response)
    {
        return response.error == NetworkErrorCode.Success && request.reqHash == LeaveRoomReqHash;
    }

    protected override RspHead CreateRoomNotFoundResponse(ReqHead request, string roomId)
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

    protected override Game001RoomFiberModule CreateRoomModule(string roomId)
    {
        return new Game001RoomFiberModule(roomId, Connections, RoomFrameRate);
    }

    protected override string CreateRoomFiberName(string roomId)
    {
        return $"Game001.RoomRoot.{roomId}";
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

    private static bool TryResolveConnectedRoomId(string? messageRoomId, string contextRoomId, out string roomId)
    {
        if (!string.IsNullOrWhiteSpace(messageRoomId))
        {
            roomId = messageRoomId;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(contextRoomId))
        {
            roomId = contextRoomId;
            return true;
        }

        roomId = string.Empty;
        return false;
    }
}

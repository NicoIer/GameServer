using Game001.Core;
using Game001.Core.Generated;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed class Game001RoomReqRspHandlers : INetworkReqRspHandlers
{
    private const string DefaultRoomId = "room-001";

    private readonly Game001RoomConnectionRegistry _connections;
    private readonly Game001RoomState _state;

    public Game001RoomReqRspHandlers(Game001RoomConnectionRegistry connections, Game001RoomState state)
    {
        _connections = connections;
        _state = state;
    }

    public void Handle(in int connectionId, in CreateRoomReq req, out CreateRoomRsp rsp, out NetworkErrorCode errorCode, out string errorMsg)
    {
        if (!TryGetContext(connectionId, out Game001RoomConnectionContext context, out errorCode, out errorMsg))
        {
            rsp = new CreateRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return;
        }

        string roomId = ResolveRoomId(req.RoomId, context.RoomId);
        RoomStateResult result = _state.CreateRoom(context.Uid, roomId);
        rsp = new CreateRoomRsp();
        FillResponse(ref rsp, ProtocolErrorCode.Success, roomId, result);
        errorCode = NetworkErrorCode.Success;
        errorMsg = string.Empty;
    }

    public void Handle(in int connectionId, in JoinRoomReq req, out JoinRoomRsp rsp, out NetworkErrorCode errorCode, out string errorMsg)
    {
        if (!TryGetContext(connectionId, out Game001RoomConnectionContext context, out errorCode, out errorMsg))
        {
            rsp = new JoinRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return;
        }

        string roomId = ResolveRoomId(req.RoomId, context.RoomId);
        RoomStateResult result = _state.JoinRoom(context.Uid, roomId);
        int error = result.Success ? ProtocolErrorCode.Success : ProtocolErrorCode.RoomNotFound;
        rsp = new JoinRoomRsp();
        FillResponse(ref rsp, error, roomId, result);
        errorCode = NetworkErrorCode.Success;
        errorMsg = string.Empty;
    }

    public void Handle(in int connectionId, in LeaveRoomReq req, out LeaveRoomRsp rsp, out NetworkErrorCode errorCode, out string errorMsg)
    {
        if (!TryGetContext(connectionId, out Game001RoomConnectionContext context, out errorCode, out errorMsg))
        {
            rsp = new LeaveRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return;
        }

        string roomId = ResolveRoomId(req.RoomId, context.RoomId);
        RoomStateResult result = _state.LeaveRoom(context.Uid, roomId);
        int error = result.Success ? ProtocolErrorCode.Success : ProtocolErrorCode.RoomNotFound;
        rsp = new LeaveRoomRsp();
        FillResponse(ref rsp, error, roomId, result);
        errorCode = NetworkErrorCode.Success;
        errorMsg = string.Empty;
    }

    public void Handle(in int connectionId, in RoomPingReq req, out RoomPingRsp rsp, out NetworkErrorCode errorCode, out string errorMsg)
    {
        if (!TryGetContext(connectionId, out Game001RoomConnectionContext context, out errorCode, out errorMsg))
        {
            rsp = new RoomPingRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return;
        }

        string roomId = ResolveRoomId(req.RoomId, context.RoomId);
        RoomStateResult result = _state.PingRoom(context.Uid, roomId);
        int error = result.Success ? ProtocolErrorCode.Success : ProtocolErrorCode.RoomNotFound;
        rsp = new RoomPingRsp();
        FillResponse(ref rsp, error, roomId, result);
        errorCode = NetworkErrorCode.Success;
        errorMsg = string.Empty;
    }

    private bool TryGetContext(int connectionId, out Game001RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg)
    {
        if (_connections.TryGet(connectionId, out context))
        {
            errorCode = NetworkErrorCode.Success;
            errorMsg = string.Empty;
            return true;
        }

        errorCode = NetworkErrorCode.InvalidArgument;
        errorMsg = $"missing room connection context connectionId={connectionId}";
        return false;
    }

    private static string ResolveRoomId(string? messageRoomId, string contextRoomId)
    {
        if (!string.IsNullOrWhiteSpace(messageRoomId))
        {
            return messageRoomId;
        }

        if (contextRoomId.Length > 0)
        {
            return contextRoomId;
        }

        return DefaultRoomId;
    }

    private static void FillResponse(ref CreateRoomRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref JoinRoomRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref LeaveRoomRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref RoomPingRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

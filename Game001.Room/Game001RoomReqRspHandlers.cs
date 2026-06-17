using Game001.Core;
using Game001.Core.Generated;
using GameServer.Core.Rooms;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed class Game001RoomReqRspHandlers : INetworkReqRspHandlers
{
    private readonly RoomConnectionRegistry _connections;
    private readonly RoomRuntimeState _state;
    private readonly RoomFrameAwaiter _frameAwaiter;

    public Game001RoomReqRspHandlers(RoomConnectionRegistry connections, RoomRuntimeState state, RoomFrameAwaiter frameAwaiter)
    {
        _connections = connections;
        _state = state;
        _frameAwaiter = frameAwaiter;
    }

    public ValueTask<(CreateRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(int connectionId, CreateRoomReq req)
    {
        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new CreateRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return new ValueTask<(CreateRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        RoomStateResult result = _state.CreateRoom(context.Uid);
        var rsp = new CreateRoomRsp();
        FillResponse(ref rsp, ProtocolErrorCode.Success, _state.RoomId, result);
        return new ValueTask<(CreateRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, string.Empty));
    }

    public ValueTask<(JoinRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(int connectionId, JoinRoomReq req)
    {
        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new JoinRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        RoomStateResult result = _state.JoinRoom(context.Uid);
        var rsp = new JoinRoomRsp();
        FillResponse(ref rsp, ProtocolErrorCode.Success, _state.RoomId, result);
        return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, string.Empty));
    }

    public ValueTask<(LeaveRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(int connectionId, LeaveRoomReq req)
    {
        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new LeaveRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        RoomStateResult result = _state.LeaveRoom(context.Uid);
        var rsp = new LeaveRoomRsp();
        FillResponse(ref rsp, ProtocolErrorCode.Success, _state.RoomId, result);
        return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, string.Empty));
    }

    public async ValueTask<(RoomPingRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(int connectionId, RoomPingReq req)
    {
        await _frameAwaiter.WaitNextFrameAsync();

        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new RoomPingRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
            return (invalidRsp, errorCode, errorMsg);
        }

        RoomStateResult result = _state.PingRoom(context.Uid);
        var rsp = new RoomPingRsp();
        FillResponse(ref rsp, ProtocolErrorCode.Success, _state.RoomId, result);
        return (rsp, NetworkErrorCode.Success, string.Empty);
    }

    private bool TryGetContext(int connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg)
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

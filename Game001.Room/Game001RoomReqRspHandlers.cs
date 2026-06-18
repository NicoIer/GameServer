using Game001.Core;
using Game001.Core.Generated;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers : IGame001Handler
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

    private static void FillResponse(ref CreateRoomRsp rsp, string roomId, RoomStateResult result)
    {
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref JoinRoomRsp rsp, string roomId, RoomStateResult result)
    {
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref LeaveRoomRsp rsp, string roomId, RoomStateResult result)
    {
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref RoomPingRsp rsp, string roomId, RoomStateResult result)
    {
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

using Game001.Core;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public ValueTask<(RoomResyncRsp rsp, NetworkErrorCode errorCode, string errorMsg)> HandleRoomResync(
        int connectionId,
        RoomResyncReq req)
    {
        if (!TryGetContext(
                connectionId,
                out RoomConnectionContext context,
                out NetworkErrorCode errorCode,
                out string errorMsg))
        {
            var invalidRsp = new RoomResyncRsp();
            return new ValueTask<(RoomResyncRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        if (!string.Equals(context.RoomId, _state.RoomId, StringComparison.Ordinal) ||
            !_state.ActiveConnectionIds.Contains(connectionId))
        {
            var inactiveRsp = new RoomResyncRsp();
            return new ValueTask<(RoomResyncRsp, NetworkErrorCode, string)>(
                (inactiveRsp, NetworkErrorCode.InvalidArgument, $"connection is not active room={_state.RoomId}"));
        }

        _state.PendingFullStateConnections.Add(connectionId);
        var rsp = new RoomResyncRsp();
        return new ValueTask<(RoomResyncRsp, NetworkErrorCode, string)>(
            (rsp, NetworkErrorCode.Success, $"resync queued room={_state.RoomId} revision={_state.WorldRevision}"));
    }
}

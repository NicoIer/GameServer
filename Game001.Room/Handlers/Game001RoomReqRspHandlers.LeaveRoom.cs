using Game001.Core;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public ValueTask<(LeaveRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> HandleLeaveRoom(
        int connectionId,
        LeaveRoomReq req)
    {
        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new LeaveRoomRsp();
            return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        string message = _room.LeaveRoom(connectionId, context.Uid);
        var rsp = new LeaveRoomRsp();
        return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, message));
    }
}

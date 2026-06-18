using Game001.Core;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public ValueTask<(JoinRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> HandleJoinRoom(
        int connectionId,
        JoinRoomReq req)
    {
        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new JoinRoomRsp();
            return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        string message = _room.JoinRoom(connectionId, context.Uid);
        var rsp = new JoinRoomRsp();
        return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, message));
    }
}

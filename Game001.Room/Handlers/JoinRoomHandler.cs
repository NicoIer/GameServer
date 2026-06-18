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

        RoomStateResult result = _state.JoinRoom(context.Uid);
        var rsp = new JoinRoomRsp();
        FillResponse(ref rsp, _state.RoomId, result);
        return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, result.Message));
    }
}

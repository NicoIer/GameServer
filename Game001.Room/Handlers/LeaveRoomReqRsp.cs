using Game001.Core;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public static partial class LeaveRoomReqRsp
    {
        public static ValueTask<(LeaveRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(
            Game001RoomReqRspHandlers self,
            int connectionId,
            LeaveRoomReq req)
        {
            if (!self.TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
            {
                var invalidRsp = new LeaveRoomRsp();
                return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
            }

            RoomStateResult result = self._state.LeaveRoom(context.Uid);
            var rsp = new LeaveRoomRsp();
            FillResponse(ref rsp, self._state.RoomId, result);
            return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, result.Message));
        }
    }
}

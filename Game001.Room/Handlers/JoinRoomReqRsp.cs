using Game001.Core;
using GameServer.Core.Rooms;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public static partial class JoinRoomReqRsp
    {
        public static ValueTask<(JoinRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(
            Game001RoomReqRspHandlers self,
            int connectionId,
            JoinRoomReq req)
        {
            if (!self.TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
            {
                var invalidRsp = new JoinRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
                return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
            }

            RoomStateResult result = self._state.JoinRoom(context.Uid);
            var rsp = new JoinRoomRsp();
            FillResponse(ref rsp, ProtocolErrorCode.Success, self._state.RoomId, result);
            return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, string.Empty));
        }
    }
}

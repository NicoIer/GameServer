using Game001.Core;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public ValueTask<(RoomPingRsp rsp, NetworkErrorCode errorCode, string errorMsg)> HandleRoomPing(
        int connectionId,
        RoomPingReq req)
    {
        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new RoomPingRsp();
            return new ValueTask<(RoomPingRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        string message = _lifecycleSystem.PingRoom(context.Uid);
        var rsp = new RoomPingRsp();
        return new ValueTask<(RoomPingRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, message));
    }
}

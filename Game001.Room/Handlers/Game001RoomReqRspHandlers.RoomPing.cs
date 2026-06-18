using Game001.Core;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public async ValueTask<(RoomPingRsp rsp, NetworkErrorCode errorCode, string errorMsg)> HandleRoomPing(
        int connectionId,
        RoomPingReq req)
    {
        await _frameAwaiter.WaitNextFrameAsync();

        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new RoomPingRsp();
            return (invalidRsp, errorCode, errorMsg);
        }

        string message = _room.PingRoom(context.Uid);
        var rsp = new RoomPingRsp();
        return (rsp, NetworkErrorCode.Success, message);
    }
}

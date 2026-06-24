using Game001.Core;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    public ValueTask<(CreateRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> HandleCreateRoom(
        int connectionId,
        CreateRoomReq req)
    {
        if (!TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
        {
            var invalidRsp = new CreateRoomRsp();
            return new ValueTask<(CreateRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
        }

        string message = _lifecycleSystem.CreateRoom(connectionId, context.Uid);
        var rsp = new CreateRoomRsp();
        return new ValueTask<(CreateRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, message));
    }
}

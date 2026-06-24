using Game001.Core;
using Game001.Core.Generated;
using Game001.Room.Systems;
using GameServer.Core.Rooms;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers : IGame001Handler
{
    private readonly RoomConnectionRegistry _connections;
    private readonly RoomLifecycleSystem _lifecycleSystem;

    public Game001RoomReqRspHandlers(RoomConnectionRegistry connections, RoomLifecycleSystem lifecycleSystem)
    {
        _connections = connections;
        _lifecycleSystem = lifecycleSystem;
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

}

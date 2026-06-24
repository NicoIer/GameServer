using Game001.Core;
using Game001.Room.Runtime;
using GameServer.Core.Rooms;
using GameServer.Core.Systems;

namespace Game001.Room.Systems;

[ExecuteAfter(typeof(RoomLifecycleSystem))]
public sealed class RoomSyncSystem : ISystem
{
    private readonly RoomPushHub _pushHub;
    private readonly Game001RoomState _state;

    public RoomSyncSystem(RoomPushHub pushHub, Game001RoomState state)
    {
        _pushHub = pushHub;
        _state = state;
    }

    public void OnCreate()
    {
    }

    public void Update(in long deltaTimeMs, in int frame, in long timeNowMs)
    {
        if (_state.PendingFullStateConnections.Count > 0)
        {
            var push = new RoomFullStatePush
            {
                Room = _state.CreateRoomInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            };
            _pushHub.SendMany(_state.PendingFullStateConnections, push);

            _state.PendingFullStateConnections.Clear();
        }
    }

    public void OnDestroy()
    {
    }
}

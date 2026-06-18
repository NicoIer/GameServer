using Game001.Core;
using Game001.Room.Runtime;
using GameServer.Core.Rooms;

namespace Game001.Room.Systems;

public sealed class RoomReplicationSystem
{
    private readonly RoomConnectionRegistry _connections;
    private readonly RoomPushHub _pushHub;
    private readonly Game001RoomState _state;

    public RoomReplicationSystem(RoomConnectionRegistry connections, RoomPushHub pushHub, Game001RoomState state)
    {
        _connections = connections;
        _pushHub = pushHub;
        _state = state;
    }

    public void SendRoomEvent(int actorConnectionId, RoomPushType type, long uid)
    {
        var push = new RoomEventPush
        {
            Type = type,
            Room = _state.CreateRoomInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            Uid = uid,
        };

        bool sentActor = false;
        List<int> roomConnectionIds = _connections.GetRoomConnectionIds(_state.RoomId);
        foreach (int connectionId in roomConnectionIds)
        {
            _pushHub.Send(connectionId, push);
            if (connectionId == actorConnectionId)
            {
                sentActor = true;
            }
        }

        if (!sentActor)
        {
            _pushHub.Send(actorConnectionId, push);
        }
    }
}

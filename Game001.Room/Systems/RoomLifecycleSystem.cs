using Game001.Core;
using Game001.Room.Runtime;

namespace Game001.Room.Systems;

public sealed class RoomLifecycleSystem
{
    private readonly Game001RoomState _state;
    private readonly RoomReplicationSystem _replication;

    public RoomLifecycleSystem(Game001RoomState state, RoomReplicationSystem replication)
    {
        _state = state;
        _replication = replication;
    }

    public string CreateRoom(int connectionId, long uid)
    {
        _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _replication.SendRoomEvent(connectionId, RoomPushType.RoomCreated, uid);
        return $"created room={_state.RoomId} players={_state.Players.Count}";
    }

    public string JoinRoom(int connectionId, long uid)
    {
        bool added = _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        if (added)
        {
            _replication.SendRoomEvent(connectionId, RoomPushType.PlayerJoined, uid);
        }

        return $"joined room={_state.RoomId} players={_state.Players.Count}";
    }

    public string LeaveRoom(int connectionId, long uid)
    {
        bool removed = _state.Players.Remove(uid);
        _state.DisconnectedPlayers.Remove(uid);
        if (removed)
        {
            _replication.SendRoomEvent(connectionId, RoomPushType.PlayerLeft, uid);
        }

        return $"left room={_state.RoomId} players={_state.Players.Count}";
    }

    public string DisconnectRoom(int connectionId, long uid)
    {
        if (_state.Players.Contains(uid) && _state.DisconnectedPlayers.Add(uid))
        {
            _replication.SendRoomEvent(connectionId, RoomPushType.PlayerDisconnected, uid);
        }

        return $"disconnected uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }

    public string PingRoom(long uid)
    {
        return $"pong uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }
}

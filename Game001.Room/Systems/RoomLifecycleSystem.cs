using Game001.Core;
using Game001.Room.Runtime;

namespace Game001.Room.Systems;

public sealed class RoomLifecycleSystem
{
    private readonly Game001RoomState _state;
    private readonly RoomSyncSystem _sync;

    public RoomLifecycleSystem(Game001RoomState state, RoomSyncSystem sync)
    {
        _state = state;
        _sync = sync;
    }

    public string CreateRoom(int connectionId, long uid)
    {
        _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _sync.MarkDirty();
        _sync.SendFullState(connectionId);
        return $"created room={_state.RoomId} players={_state.Players.Count}";
    }

    public string JoinRoom(int connectionId, long uid)
    {
        bool added = _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        if (added)
        {
            _sync.MarkDirty();
        }

        _sync.SendFullState(connectionId);
        return $"joined room={_state.RoomId} players={_state.Players.Count}";
    }

    public string LeaveRoom(int connectionId, long uid)
    {
        bool removed = _state.Players.Remove(uid);
        _state.DisconnectedPlayers.Remove(uid);
        if (removed)
        {
            _sync.MarkDirty();
        }

        return $"left room={_state.RoomId} players={_state.Players.Count}";
    }

    public string DisconnectRoom(int connectionId, long uid)
    {
        if (_state.Players.Contains(uid) && _state.DisconnectedPlayers.Add(uid))
        {
            _sync.MarkDirty();
        }

        return $"disconnected uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }

    public string PingRoom(long uid)
    {
        return $"pong uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }
}

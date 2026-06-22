using Game001.Room.Runtime;
using GameServer.Core.Rooms;

namespace Game001.Room.Systems;

public sealed class RoomLifecycleSystem
{
    private const long DisconnectedEmptyTimeoutMs = 30_000;

    private readonly Game001RoomState _state;
    private readonly RoomSyncSystem _sync;

    public RoomLifecycleSystem(Game001RoomState state, RoomSyncSystem sync)
    {
        _state = state;
        _sync = sync;
    }

    public string CreateRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _state.DisconnectedPlayerTimesMs.Remove(uid);
        _state.SetActive(timeNowMs);
        // _sync.MarkDirty();
        _sync.SendFullState(connectionId);
        return $"created room={_state.RoomId} players={_state.Players.Count}";
    }

    public string JoinRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        bool added = _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _state.DisconnectedPlayerTimesMs.Remove(uid);
        _state.SetActive(timeNowMs);
        if (added)
        {
            // _sync.MarkDirty();
        }

        _sync.SendFullState(connectionId);
        return $"joined room={_state.RoomId} players={_state.Players.Count}";
    }

    public string LeaveRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        bool removed = _state.Players.Remove(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _state.DisconnectedPlayerTimesMs.Remove(uid);
        if (removed)
        {
            // _sync.MarkDirty();
        }

        RefreshEmptyState(timeNowMs);
        return $"left room={_state.RoomId} players={_state.Players.Count}";
    }

    public string DisconnectRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        if (_state.Players.Contains(uid) && _state.DisconnectedPlayers.Add(uid))
        {
            _state.DisconnectedPlayerTimesMs[uid] = timeNowMs;
            // _sync.MarkDirty();
        }

        return $"disconnected uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }

    public string PingRoom(long uid)
    {
        return $"pong uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }

    public void Update(long timeNowMs)
    {
        if (_state.LifecycleState == RoomLifecycleState.Closing ||
            _state.LifecycleState == RoomLifecycleState.Closed)
        {
            return;
        }

        RefreshDisconnectedTimeout(timeNowMs);
        RefreshEmptyState(timeNowMs);
    }

    public bool ShouldCloseRoom(long timeNowMs)
    {
        return _state.LifecycleState == RoomLifecycleState.Empty ||
               _state.LifecycleState == RoomLifecycleState.Closing ||
               _state.LifecycleState == RoomLifecycleState.Closed;
    }

    public void BeginCloseRoom(long timeNowMs)
    {
        _state.SetClosing(timeNowMs);
    }

    public void CloseRoom(long timeNowMs)
    {
        _state.SetClosed(timeNowMs);
    }

    private void RefreshEmptyState(long timeNowMs)
    {
        if (_state.Players.Count == 0)
        {
            _state.SetEmpty(timeNowMs);
        }
    }

    private void RefreshDisconnectedTimeout(long timeNowMs)
    {
        if (_state.Players.Count == 0 || _state.DisconnectedPlayers.Count != _state.Players.Count)
        {
            return;
        }

        long disconnectedSinceMs = 0;
        foreach (long uid in _state.Players)
        {
            if (!_state.DisconnectedPlayerTimesMs.TryGetValue(uid, out long playerDisconnectedTimeMs))
            {
                return;
            }

            if (playerDisconnectedTimeMs > disconnectedSinceMs)
            {
                disconnectedSinceMs = playerDisconnectedTimeMs;
            }
        }

        if (timeNowMs - disconnectedSinceMs < DisconnectedEmptyTimeoutMs)
        {
            return;
        }

        foreach (long uid in _state.DisconnectedPlayers)
        {
            _state.Players.Remove(uid);
            _state.DisconnectedPlayerTimesMs.Remove(uid);
        }

        _state.DisconnectedPlayers.Clear();
    }
}

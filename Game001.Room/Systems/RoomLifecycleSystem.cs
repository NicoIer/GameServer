using Game001.Room.Runtime;
using GameServer.Core.Rooms;
using GameServer.Core.Systems;

namespace Game001.Room.Systems;

public sealed class RoomLifecycleSystem : ISystem
{
    private const long DisconnectedEmptyTimeoutMs = 30_000;

    private readonly Game001RoomState _state;

    public RoomLifecycleSystem(Game001RoomState state)
    {
        _state = state;
    }

    public void OnCreate()
    {
    }

    public string CreateRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _state.DisconnectedPlayerTimesMs.Remove(uid);
        _state.UpdatePlayerCount();
        _state.SetActive(timeNowMs);
        _state.PendingFullStateConnections.Add(connectionId);
        return $"created room={_state.RoomId} players={_state.Players.Count}";
    }

    public string JoinRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        _state.Players.Add(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _state.DisconnectedPlayerTimesMs.Remove(uid);
        _state.UpdatePlayerCount();
        _state.SetActive(timeNowMs);
        _state.PendingFullStateConnections.Add(connectionId);
        return $"joined room={_state.RoomId} players={_state.Players.Count}";
    }

    public string LeaveRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        _state.Players.Remove(uid);
        _state.DisconnectedPlayers.Remove(uid);
        _state.DisconnectedPlayerTimesMs.Remove(uid);
        _state.UpdatePlayerCount();
        RefreshEmptyState(timeNowMs);
        return $"left room={_state.RoomId} players={_state.Players.Count}";
    }

    public string DisconnectRoom(int connectionId, long uid)
    {
        long timeNowMs = Environment.TickCount64;
        if (_state.Players.Contains(uid) && _state.DisconnectedPlayers.Add(uid))
        {
            _state.DisconnectedPlayerTimesMs[uid] = timeNowMs;
        }

        return $"disconnected uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }

    public string PingRoom(long uid)
    {
        return $"pong uid={uid} room={_state.RoomId} players={_state.Players.Count}";
    }

    public void Update(in long deltaTimeMs, in int frame, in long timeNowMs)
    {
        if (_state.LifecycleState == RoomLifecycleState.Closing ||
            _state.LifecycleState == RoomLifecycleState.Closed)
        {
            return;
        }

        RefreshDisconnectedTimeout(timeNowMs);
        RefreshEmptyState(timeNowMs);
    }

    public void OnDestroy()
    {
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
        _state.UpdatePlayerCount();
    }
}

using Game001.Room.Systems;
using GameServer.Core.Rooms;

namespace Game001.Room.Runtime;

public sealed class Game001Room
{
    private readonly RoomLifecycleSystem _lifecycleSystem;
    private readonly RoomSyncSystem _syncSystem;

    public Game001Room(string roomId, RoomPushHub pushHub)
    {
        State = new Game001RoomState(roomId);
        _syncSystem = new RoomSyncSystem(pushHub, State);
        _lifecycleSystem = new RoomLifecycleSystem(State, _syncSystem);
    }

    public Game001RoomState State { get; }
    public RoomLifecycleState LifecycleState => State.LifecycleState;

    public string CreateRoom(int connectionId, long uid)
    {
        return _lifecycleSystem.CreateRoom(connectionId, uid);
    }

    public string JoinRoom(int connectionId, long uid)
    {
        return _lifecycleSystem.JoinRoom(connectionId, uid);
    }

    public string LeaveRoom(int connectionId, long uid)
    {
        return _lifecycleSystem.LeaveRoom(connectionId, uid);
    }

    public string PingRoom(long uid)
    {
        return _lifecycleSystem.PingRoom(uid);
    }

    public string DisconnectRoom(int connectionId, long uid)
    {
        return _lifecycleSystem.DisconnectRoom(connectionId, uid);
    }

    public void Update(long timeNowMs, int frame)
    {
        State.SetFrame(timeNowMs, frame);
        _lifecycleSystem.Update(timeNowMs);
        _syncSystem.Update(timeNowMs, frame);
    }

    public bool ShouldCloseRoom(long timeNowMs)
    {
        return _lifecycleSystem.ShouldCloseRoom(timeNowMs);
    }

    public void BeginCloseRoom(long timeNowMs)
    {
        _lifecycleSystem.BeginCloseRoom(timeNowMs);
    }

    public void CloseRoom(long timeNowMs)
    {
        _lifecycleSystem.CloseRoom(timeNowMs);
    }
}

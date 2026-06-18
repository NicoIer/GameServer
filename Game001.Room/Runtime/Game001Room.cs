using Game001.Room.Systems;
using GameServer.Core.Rooms;

namespace Game001.Room.Runtime;

public sealed class Game001Room
{
    private readonly RoomLifecycleSystem _lifecycleSystem;

    public Game001Room(string roomId, RoomConnectionRegistry connections, RoomPushHub pushHub)
    {
        State = new Game001RoomState(roomId);
        var replicationSystem = new RoomReplicationSystem(connections, pushHub, State);
        _lifecycleSystem = new RoomLifecycleSystem(State, replicationSystem);
    }

    public Game001RoomState State { get; }

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
        State.Update(timeNowMs, frame);
    }
}

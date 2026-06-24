using Game001.Room.Systems;
using GameServer.Core.Rooms;
using GameServer.Core.Systems;

namespace Game001.Room.Runtime;

public sealed class Game001Room : IWorld
{
    public Game001Room(string roomId, RoomPushHub pushHub)
    {
        State = new Game001RoomState(roomId);
        var syncSystem = new RoomSyncSystem(pushHub, State);
        var lifecycleSystem = new RoomLifecycleSystem(State);
        Systems = new SystemGroup(lifecycleSystem, syncSystem);
        Systems.OnCreate();
    }

    public Game001RoomState State { get; }
    public SystemGroup Systems { get; }
    public RoomLifecycleState LifecycleState => State.LifecycleState;

    public void Update(long timeNowMs, int frame)
    {
        long lastUpdateTimeMs = State.LastUpdateTimeMs;
        long deltaTimeMs = lastUpdateTimeMs == 0 ? 0 : timeNowMs - lastUpdateTimeMs;
        State.SetFrame(timeNowMs, frame);
        Systems.Update(in deltaTimeMs, in frame, in timeNowMs);
    }

    public void Destroy()
    {
        Systems.OnDestroy();
    }
}

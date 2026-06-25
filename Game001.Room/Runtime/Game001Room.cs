using Game001.Room.Systems;
using Friflo.Engine.ECS;
using GameServer.Core.Rooms;
using GameServer.Core.Systems;
using CoreSystemGroup = GameServer.Core.Systems.SystemGroup;
using FrifloSystemRoot = Friflo.Engine.ECS.Systems.SystemRoot;

namespace Game001.Room.Runtime;

public sealed class Game001Room : IWorld
{
    public Game001Room(string roomId, RoomPushHub pushHub)
    {
        State = new Game001RoomState(roomId);
        Game001RoomEcsSystems.Configure(State.EcsSystems);
        var syncSystem = new RoomSyncSystem(pushHub, State);
        var lifecycleSystem = new RoomLifecycleSystem(State);
        var frifloSystemRunner = new FrifloSystemRunnerSystem(State.EcsSystems);
        Systems = new CoreSystemGroup(lifecycleSystem, frifloSystemRunner, syncSystem);
    }

    public Game001RoomState State { get; }
    public EntityStore World => State.Entities;
    public FrifloSystemRoot EcsSystems => State.EcsSystems;
    public CoreSystemGroup Systems { get; }
    public RoomLifecycleState LifecycleState => State.LifecycleState;

    public void Create()
    {
        Systems.OnCreate();
    }

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
        State.Destroy();
    }
}

using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using GameServer.Core.Systems;

namespace Game001.Room.Systems;

[ExecuteAfter(typeof(RoomLifecycleSystem))]
[ExecuteBefore(typeof(RoomSyncSystem))]
public sealed class FrifloSystemRunnerSystem : ISystem
{
    private readonly SystemRoot _root;
    private long _startTimeMs;

    public FrifloSystemRunnerSystem(SystemRoot root)
    {
        _root = root;
    }

    public SystemRoot Root => _root;

    public void OnCreate()
    {
    }

    public void Update(in long deltaTimeMs, in int frame, in long timeNowMs)
    {
        if (_startTimeMs == 0)
        {
            _startTimeMs = timeNowMs;
        }

        float deltaTimeSeconds = deltaTimeMs / 1000f;
        float timeSeconds = (timeNowMs - _startTimeMs) / 1000f;
        var tick = new UpdateTick(deltaTimeSeconds, timeSeconds);
        _root.Update(tick);
    }

    public void OnDestroy()
    {
    }
}

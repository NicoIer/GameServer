using Friflo.Engine.ECS;
using GameServer.Core.Ecs;
using MemoryPack;

namespace Game001.Core.Ecs;

[MemoryPackable]
[EcsReplicatedComponent]
public partial struct RoomPlayerComponent : IComponent
{
    public long Uid;
}

[MemoryPackable]
[EcsReplicatedComponent]
public partial struct RoomDisconnectedComponent : IComponent
{
    public long TimeMs;
}

using Friflo.Engine.ECS;

namespace Game001.Room.Runtime.Components;

public struct RoomPlayerComponent : IComponent
{
    public long Uid;
}

public struct RoomDisconnectedComponent : IComponent
{
    public long TimeMs;
}

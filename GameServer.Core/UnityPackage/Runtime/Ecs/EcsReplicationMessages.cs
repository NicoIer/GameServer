using System;
using MemoryPack;

namespace GameServer.Core.Ecs
{
    [MemoryPackable]
    public partial struct EcsEntitySnapshot
    {
        public int EntityId;
        public int ParentEntityId;
        public ArraySegment<EcsComponentSnapshot> Components;
    }

    [MemoryPackable]
    public partial struct EcsComponentSnapshot
    {
        public ushort ComponentTypeId;
        public ArraySegment<byte> Payload;
    }

    [MemoryPackable]
    public partial struct EcsEntityChange
    {
        public int EntityId;
        public int ParentEntityId;
        public EcsChangeKind Kind;
    }

    [MemoryPackable]
    public partial struct EcsComponentChange
    {
        public int EntityId;
        public ushort ComponentTypeId;
        public EcsChangeKind Kind;
        public ArraySegment<byte> Payload;
    }

    public enum EcsChangeKind
    {
        Create,
        Delete,
        Add,
        Update,
        Remove,
        Reparent,
    }
}

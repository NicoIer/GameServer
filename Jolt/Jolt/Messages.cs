using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using MemoryPack;
using Network;


namespace GameCore.Jolt
{
    // public interface INetworkEntity
    // {
    //     /// <summary>
    //     /// 实体拥有者的ID
    //     /// 0表示服务器
    //     /// </summary>
    //     public int ownerId { get; } // player id
    //
    //     public uint entityId { get; } // entity id
    //     public byte worldId { get; } // world id
    // }


    [MemoryPackable]
    public partial struct WorldData : INetworkMessage
    {
        public byte worldId;
        public long frameCount;
        public long timeStamp;
        public Vector3 gravity;
        public ArraySegment<BodyData> bodies;
    }


    [MemoryPackable]
    public partial struct BodyData
        // : INetworkEntity
    {
        public int ownerId { get; set; } // player id
        public uint entityId { get; set; } // entity id -> jolt bodyId
        // public byte worldId { get; set; } // world id

        public BodyType bodyType;

        [MemoryPackIgnore] public bool isRigid => bodyType == BodyType.Rigid;
        [MemoryPackIgnore] public bool isSoft => bodyType == BodyType.Soft;

        public bool isActive;
        public bool isStatic;
        public bool isKinematic;
        public bool isDynamic;

        /// <summary>
        /// Same To PhysX isTrigger
        /// </summary>
        public bool isSensor;

        [MemoryPackIgnore] public bool isTrigger => isSensor;

        public ushort objectLayer;
        public byte broadPhaseLayer;

        public bool allowSleeping;

        public float friction;
        public float restitution;

        public Vector3 position;
        public Quaternion rotation;

        public Vector3 centerOfMass;

        public Vector3 linearVelocity;
        public Vector3 angularVelocity;

        public IShapeData shapeData;
    }


    [MemoryPackable]
    [MemoryPackUnion(0, typeof(BoxShapeData))]
    public partial interface IShapeData
    {
        // public ShapeType type;
        // public ShapeSubType subType;
        // public float innerRadius;
        // public Vector3 scale;

        // public float volume;

        // public Vector3 centerOfMass;
        // public BoundingBox boundingBox;
        // public ArraySegment<byte> data;
    }

    [MemoryPackable]
    public partial struct BoxShapeData : IShapeData
    {
        public Vector3 halfExtents;

        public BoxShapeData(Vector3 halfExtents)
        {
            this.halfExtents = halfExtents;
        }
    }

    [MemoryPackable]
    public partial struct SphereShapeData : IShapeData
    {
        public float radius;

        public SphereShapeData(float radius)
        {
            this.radius = radius;
        }
    }
}
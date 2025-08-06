using System.Numerics;
using MemoryPack;
using Network;

namespace GameCore.Physics
{
    // public interface IBodySpawnCmd
    // {
    //     public IShapeData GetShapeData();
    // }
    //
    // [MemoryPackable]
    // public partial struct CmdSpawnBox : INetworkMessage, IBodySpawnCmd
    // {
    //     public Vector3 halfExtents;
    //     public Vector3 position;
    //     public Quaternion rotation;
    //     public MotionType motionType;
    //     public Activation activation;
    //     public ObjectLayers objectLayer;
    //
    //     public readonly IShapeData GetShapeData()
    //     {
    //         return new BoxShapeData(halfExtents);
    //     }
    // }
    //
    // [MemoryPackable]
    // public partial struct CmdSpawnPlane : INetworkMessage, IBodySpawnCmd
    // {
    //     public Vector3 position;
    //     public Quaternion rotation;
    //     public MotionType motionType;
    //     public Vector3 normal;
    //     public float distance;
    //     public float halfExtent;
    //     public Activation activation;
    //     public ObjectLayers objectLayer;
    //
    //     public readonly IShapeData GetShapeData()
    //     {
    //         return new PlaneShapeData(halfExtent, normal, distance);
    //     }
    // }
    //
    // [MemoryPackable]
    // public partial struct CmdSpawnSphere : INetworkMessage, IBodySpawnCmd
    // {
    //     public Vector3 position;
    //     public Quaternion rotation;
    //     public MotionType motionType;
    //     public Activation activation;
    //     public ObjectLayers objectLayer;
    //     public float radius;
    //
    //     public readonly IShapeData GetShapeData()
    //     {
    //         return new SphereShapeData(radius);
    //     }
    // }

    [MemoryPackable]
    public partial struct CmdSpawnBody : INetworkMessage
        // , IBodySpawnCmd
    {
        public ShapeDataPacket shapeDataPacket;
        public Vector3 position;
        public Quaternion rotation;
        public MotionType motionType;
        public Activation activation;
        public ObjectLayers objectLayer;

        public readonly IShapeData GetShapeData()
        {
            return ShapeDataPacket.Deserialize(shapeDataPacket);
        }
    }

    [MemoryPackable]
    public partial struct CmdDestroy : INetworkMessage
    {
        public uint entityId;
    }

    [MemoryPackable]
    public partial struct CmdBodyState : INetworkMessage
    {
        public uint entityId;
        public Activation activation;
        public bool isActive;
        public Vector3? position;
        public Quaternion? rotation;
        public Vector3? linearVelocity;
        public Vector3? angularVelocity;
    }
}
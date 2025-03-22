using System.Numerics;
using MemoryPack;
using Network;

namespace GameCore.Jolt
{
    [MemoryPackable]
    public partial struct CmdSpawnBox : INetworkMessage
    {
        public Vector3 halfExtents;
        public Vector3 position;
        public Quaternion rotation;
        public MotionType motionType;
        public Activation activation;
        public ObjectLayers objectLayer;
    }

    [MemoryPackable]
    public partial struct CmdSpawnPlane : INetworkMessage
    {
        public Vector3 position;
        public Quaternion rotation;
        public MotionType motionType;
        public Vector3 normal;
        public float distance;
        public float halfExtent;
        public Activation activation;
        public ObjectLayers objectLayer;
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
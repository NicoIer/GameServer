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
}
using MemoryPack;
using UnityToolkit.MathTypes;

namespace Network.Position
{
    [MemoryPackable]
    public partial struct PostionMessage : INetworkMessage
    {
        public int entityId;
        public Vector3 position;
    }
}
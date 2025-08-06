using MemoryPack;
using Network;

namespace GameCore.Physics
{
    [MemoryPackable]
    public partial struct RpcContactAdded : INetworkMessage
    {
        public uint body1;
        public uint body2;

        public RpcContactAdded(uint body1, uint body2)
        {
            this.body1 = body1;
            this.body2 = body2;
        }
    }


    [MemoryPackable]
    public partial struct RpcContactPersisted : INetworkMessage
    {
        public uint body1;
        public uint body2;

        public RpcContactPersisted(uint body1, uint body2)
        {
            this.body1 = body1;
            this.body2 = body2;
        }
    }

    [MemoryPackable]
    public partial struct RpcContactRemoved : INetworkMessage
    {
        public uint body1;
        public uint body2;

        public RpcContactRemoved(uint body1, uint body2)
        {
            this.body1 = body1;
            this.body2 = body2;
        }
    }
}
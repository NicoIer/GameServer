using MemoryPack;
using Network;

namespace Soccer
{
    [MemoryPackable]
    public partial struct RpcPlayerGoal : INetworkMessage
    {
        public IdentifierEnum identifier;

        public RpcPlayerGoal(IdentifierEnum identifier)
        {
            this.identifier = identifier;
        }
    }
}
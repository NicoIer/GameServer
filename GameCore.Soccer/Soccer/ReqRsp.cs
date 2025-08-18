using MemoryPack;
using Network;
using Soccer;

namespace GameCore.Soccer
{
    [MemoryPackable]
    public partial struct ReqJoinGame : INetworkReq
    {
        public string playerName;

        public ReqJoinGame(string playerName)
        {
            this.playerName = playerName;
        }
    }

    [MemoryPackable]
    public partial struct RspJoinGame : INetworkRsp
    {
        public IdentifierEnum identifier;

        public RspJoinGame(IdentifierEnum identifier)
        {
            this.identifier = identifier;
        }
    }
}
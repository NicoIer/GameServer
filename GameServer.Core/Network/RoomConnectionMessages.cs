using MemoryPack;
using Network;

namespace GameServer.Core.Network;

[MemoryPackable]
public partial struct RoomHandshakeReq : INetworkReq
{
    public string ConnectTicket;
}

[MemoryPackable]
public partial struct RoomHandshakeRsp : INetworkRsp
{
    public long Uid;
}

[MemoryPackable]
public partial struct RoomConnectReq : INetworkReq
{
    public string RoomId;
}

[MemoryPackable]
public partial struct RoomConnectRsp : INetworkRsp
{
    public string RoomId;
}

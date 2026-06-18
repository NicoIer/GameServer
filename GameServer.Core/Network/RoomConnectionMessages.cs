using MemoryPack;
using Network;

namespace GameServer.Core.Network;

[MemoryPackable]
[NetworkRequest(typeof(RoomHandshakeRsp))]
public partial struct RoomHandshakeReq : INetworkReq
{
    public string ConnectTicket;
}

[MemoryPackable]
public partial struct RoomHandshakeRsp : INetworkRsp
{
    public int Error;
    public long Uid;
    public string Message;
}

[MemoryPackable]
[NetworkRequest(typeof(RoomConnectRsp))]
public partial struct RoomConnectReq : INetworkReq
{
    public string RoomId;
}

[MemoryPackable]
public partial struct RoomConnectRsp : INetworkRsp
{
    public int Error;
    public bool Success;
    public string Message;
    public string RoomId;
}

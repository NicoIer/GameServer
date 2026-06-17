using MemoryPack;

namespace GameServer.Core.Network;

[MemoryPackable]
public partial struct RoomConnectRequest : IGameMessage
{
    public string ConnectTicket;
    public string RoomId;
}

[MemoryPackable]
public partial struct RoomConnectionReply : IGameMessage
{
    public int Error;
    public long Uid;
    public string RoomId;
    public string Message;
}

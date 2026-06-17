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

[MemoryPackable]
public partial struct CreateRoomRequest : IGameMessage
{
    public string RoomId;
}

[MemoryPackable]
public partial struct JoinRoomRequest : IGameMessage
{
    public string RoomId;
}

[MemoryPackable]
public partial struct LeaveRoomRequest : IGameMessage
{
    public string RoomId;
}

[MemoryPackable]
public partial struct RoomPingRequest : IGameMessage
{
    public string RoomId;
    public long ClientTimeMs;
}

[MemoryPackable]
public partial struct RoomCommandReply : IGameMessage
{
    public int Error;
    public bool Success;
    public string Message;
    public string RoomId;
    public int PlayerCount;
    public long ServerTimeMs;
}

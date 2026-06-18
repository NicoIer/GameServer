using GameServer.Core.Network;
using GameServer.Core.Rooms;
using MemoryPack;
using Network;

namespace Game001.Core;

[MemoryPackable]
public partial struct RoomInfo
{
    public string RoomId;
    public int PlayerCount;
    public int Frame;
    public long ServerTimeMs;
}

[MemoryPackable]
[NetworkRequest(typeof(CreateRoomRsp))]
public partial struct CreateRoomReq : INetworkReq
{
    public string RoomId;
}

[MemoryPackable]
public partial struct CreateRoomRsp : INetworkRsp
{
}

[MemoryPackable]
[NetworkRequest(typeof(JoinRoomRsp))]
public partial struct JoinRoomReq : INetworkReq
{
    public string RoomId;
}

[MemoryPackable]
public partial struct JoinRoomRsp : INetworkRsp
{
}

[MemoryPackable]
[NetworkRequest(typeof(LeaveRoomRsp))]
public partial struct LeaveRoomReq : INetworkReq
{
    public string RoomId;
}

[MemoryPackable]
public partial struct LeaveRoomRsp : INetworkRsp
{
}

[MemoryPackable]
[NetworkRequest(typeof(RoomPingRsp))]
public partial struct RoomPingReq : INetworkReq
{
    public string RoomId;
    public long ClientTimeMs;
}

[MemoryPackable]
public partial struct RoomPingRsp : INetworkRsp
{
}

public enum RoomPushType
{
    RoomCreated,
    PlayerJoined,
    PlayerLeft,
    PlayerDisconnected,
}

[MemoryPackable]
public partial struct RoomEventPush : IRoomPush
{
    public RoomPushType Type;
    public RoomInfo Room;
    public long Uid;
}

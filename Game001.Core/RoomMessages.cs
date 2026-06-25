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
public partial struct PatchMessage
{
    public int sourceFrameId;
    public int targetFrameId;
    public byte[] patch;
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

[MemoryPackable]
public partial struct RoomFullStatePush : IRoomPush
{
    public RoomInfo Room;
    public long[] Players;
    public long[] DisconnectedPlayers;
    public EcsEntitySnapshot[] Entities;
}

[MemoryPackable]
public partial struct RoomDiffStatePush : IRoomPush
{
    public RoomInfo Room;
    public int SourceFrame;
    public int TargetFrame;
    public EcsEntityChange[] EntityChanges;
    public EcsComponentChange[] ComponentChanges;
}

[MemoryPackable]
public partial struct RoomZstdDiffPush : IRoomPush
{
    public PatchMessage patchMessage;
}

[MemoryPackable]
public partial struct EcsEntitySnapshot
{
    public int EntityId;
    public EcsComponentSnapshot[] Components;
}

[MemoryPackable]
public partial struct EcsComponentSnapshot
{
    public ushort ComponentTypeId;
    public byte[] Payload;
}

[MemoryPackable]
public partial struct EcsEntityChange
{
    public int EntityId;
    public EcsChangeKind Kind;
}

[MemoryPackable]
public partial struct EcsComponentChange
{
    public int EntityId;
    public ushort ComponentTypeId;
    public EcsChangeKind Kind;
    public byte[] Payload;
}

public enum EcsChangeKind
{
    Create,
    Delete,
    Add,
    Update,
    Remove,
}

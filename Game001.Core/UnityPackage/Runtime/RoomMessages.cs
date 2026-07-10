using System;
using GameServer.Core.Ecs;
using GameServer.Core.Network;
using GameServer.Core.Rooms;
using MemoryPack;
using Network;

namespace Game001.Core
{
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
        public ArraySegment<byte> patch;
    }

    [MemoryPackable]
    public partial struct RoomListItem
    {
        public string RoomId;
        public int PlayerCount;
        public int ConnectionCount;
        public RoomLifecycleState LifecycleState;
    }

    [MemoryPackable]
    public partial struct ListRoomsReq : INetworkReq
    {
    }

    [MemoryPackable]
    public partial struct ListRoomsRsp : INetworkRsp
    {
        public ArraySegment<RoomListItem> Rooms;
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
    [NetworkRequest(typeof(RoomResyncRsp))]
    public partial struct RoomResyncReq : INetworkReq
    {
        public string RoomId;
    }

    [MemoryPackable]
    public partial struct RoomResyncRsp : INetworkRsp
    {
    }

    [MemoryPackable]
    public partial struct RoomFullStatePush : IRoomPush
    {
        public RoomInfo Room;
        public long WorldRevision;
        public ArraySegment<long> Players;
        public ArraySegment<long> DisconnectedPlayers;
        public ArraySegment<EcsEntitySnapshot> Entities;
    }

    [MemoryPackable]
    public partial struct RoomDiffStatePush : IRoomPush
    {
        public RoomInfo Room;
        public long SourceRevision;
        public long TargetRevision;
        public ArraySegment<EcsEntityChange> EntityChanges;
        public ArraySegment<EcsComponentChange> ComponentChanges;
    }

    [MemoryPackable]
    public partial struct EcsDirtySet
    {
        public long SourceRevision;
        public long TargetRevision;
        public ArraySegment<EcsEntityChange> EntityChanges;
        public ArraySegment<EcsComponentChange> ComponentChanges;

        [MemoryPackIgnore]
        public bool HasChanges => EntityChanges.Count > 0 || ComponentChanges.Count > 0;
    }

    [MemoryPackable]
    public partial struct RoomZstdDiffPush : IRoomPush
    {
        public PatchMessage patchMessage;
    }

}

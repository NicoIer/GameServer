using System;
using Network;

namespace GameServer.Core.Rooms
{
    public enum RoomRequestConnectionAction
    {
        None,
        BindRoom,
        ClearRoom,
    }

    public enum RoomRequestRouteKind
    {
        Room,
        Worker,
    }

    public enum RoomRequestRoomIdSource
    {
        None,
        Message,
        BoundConnection,
        MessageOrBoundConnection,
        MessageOrBoundConnectionOrDefault,
    }

    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public sealed class RoomRequestRouteAttribute : Attribute
    {
        public RoomRequestRouteKind Kind { get; }
        public RoomRequestRoomIdSource RoomIdSource { get; }
        public string RoomIdMemberName { get; set; } = "RoomId";
        public string DefaultRoomId { get; set; } = string.Empty;
        public bool CanCreateRoom { get; set; }
        public RoomRequestConnectionAction SuccessConnectionAction { get; set; }
        public ErrorCode RoomNotFoundErrorCode { get; set; } = ErrorCode.InvalidArgument;

        public RoomRequestRouteAttribute(
            RoomRequestRouteKind kind,
            RoomRequestRoomIdSource roomIdSource = RoomRequestRoomIdSource.None)
        {
            Kind = kind;
            RoomIdSource = roomIdSource;
        }
    }
}

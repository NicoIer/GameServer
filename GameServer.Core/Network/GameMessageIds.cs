namespace GameServer.Core.Network;

public static class GameMessageIds
{
    public const ushort Game001RoomConnectRequest = 100;
    public const ushort Game001RoomConnectionReply = 101;
    public const ushort Game001RoomCreateRequest = 1;
    public const ushort Game001RoomJoinRequest = 2;
    public const ushort Game001RoomLeaveRequest = 3;
    public const ushort Game001RoomPingRequest = 1001;
    public const ushort Game001RoomReply = 9001;
}

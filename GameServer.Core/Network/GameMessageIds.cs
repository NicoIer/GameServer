namespace GameServer.Core.Network;

public static class GameMessageIds
{
    public const ushort RoomConnectRequest = 100;
    public const ushort RoomConnectionReply = 101;
    public const ushort Game001RoomConnectRequest = RoomConnectRequest;
    public const ushort Game001RoomConnectionReply = RoomConnectionReply;
}

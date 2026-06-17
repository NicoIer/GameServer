using MemoryPack;

namespace GameServer.Core.Network;

[MemoryPackable]
public partial struct GamePacket : IGameMessage
{
    public ushort MessageId;
    public byte[] Payload;

    public GamePacket(ushort messageId, byte[] payload)
    {
        MessageId = messageId;
        Payload = payload;
    }
}

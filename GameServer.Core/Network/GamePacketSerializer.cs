using Google.Protobuf;
using MemoryPack;

namespace GameServer.Core.Network;

public static class GamePacketSerializer
{
    public static GamePacket Pack<T>(ushort messageId, in T message)
        where T : IGameMessage
    {
        byte[] payload = MemoryPackSerializer.Serialize(message);
        return new GamePacket(messageId, payload);
    }

    public static ByteString PackToByteString<T>(ushort messageId, in T message)
        where T : IGameMessage
    {
        GamePacket packet = Pack(messageId, message);
        return ToByteString(packet);
    }

    public static T Unpack<T>(in GamePacket packet)
        where T : IGameMessage
    {
        return MemoryPackSerializer.Deserialize<T>(packet.Payload);
    }

    public static ByteString ToByteString(in GamePacket packet)
    {
        byte[] data = MemoryPackSerializer.Serialize(packet);
        return ByteString.CopyFrom(data);
    }

    public static GamePacket FromByteString(ByteString data)
    {
        return MemoryPackSerializer.Deserialize<GamePacket>(data.ToByteArray());
    }
}

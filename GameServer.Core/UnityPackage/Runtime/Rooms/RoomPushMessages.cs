using System;
using MemoryPack;
using Network;

namespace GameServer.Core.Rooms
{
    public interface IRoomPush
    {
    }

    [MemoryPackable]
    public partial struct RoomPushHead : INetworkMessage
    {
        public readonly ushort PushHash;
        public readonly ArraySegment<byte> Payload;

        public RoomPushHead(in ushort pushHash, in ArraySegment<byte> payload)
        {
            PushHash = pushHash;
            Payload = payload;
        }
    }
}

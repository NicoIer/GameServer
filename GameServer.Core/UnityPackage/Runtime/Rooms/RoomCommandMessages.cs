using System;
using MemoryPack;
using Network;

namespace GameServer.Core.Rooms
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public sealed class RoomCommandAttribute : Attribute
    {
    }

    public interface IRoomCommand
    {
    }

    [MemoryPackable]
    public partial struct RoomCommandHead : INetworkMessage
    {
        public readonly ushort CommandHash;
        public readonly ArraySegment<byte> Payload;

        public RoomCommandHead(ushort commandHash, ArraySegment<byte> payload)
        {
            CommandHash = commandHash;
            Payload = payload;
        }
    }
}

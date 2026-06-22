using System.Collections.Concurrent;
using MemoryPack;
using Network;
using UnityToolkit;

namespace GameServer.Core.Rooms;

public interface IRoomPush
{
}

[MemoryPackable]
public partial struct RoomPushHead : INetworkMessage
{
    public ushort PushHash;
    public ArraySegment<byte> Payload;
}

public sealed class RoomPushHub
{
    private readonly ConcurrentDictionary<int, Action<RoomPushHead>> _senders = new();

    public void Register(int connectionId, Action<RoomPushHead> sender)
    {
        _senders[connectionId] = sender;
    }

    public void Unregister(int connectionId)
    {
        _senders.TryRemove(connectionId, out _);
    }

    public void Send<TPush>(int connectionId, TPush push)
        where TPush : IRoomPush
    {
        if (!_senders.TryGetValue(connectionId, out Action<RoomPushHead>? sender))
        {
            return;
        }

        var head = new RoomPushHead
        {
            PushHash = TypeId<TPush>.stableId16,
            Payload = MemoryPackSerializer.Serialize(push),
        };
        sender(head);
    }

    public void SendMany<TPush>(IEnumerable<int> connectionIds, TPush push)
        where TPush : IRoomPush
    {
        var head = new RoomPushHead
        {
            PushHash = TypeId<TPush>.stableId16,
            Payload = MemoryPackSerializer.Serialize(push),
        };

        foreach (int connectionId in connectionIds)
        {
            if (_senders.TryGetValue(connectionId, out Action<RoomPushHead>? sender))
            {
                sender(head);
            }
        }
    }
}

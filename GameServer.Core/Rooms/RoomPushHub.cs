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
    public readonly ushort PushHash;
    public readonly ArraySegment<byte> Payload;

    public RoomPushHead(in ushort pushHash, in ArraySegment<byte> payload)
    {
        PushHash = pushHash;
        Payload = payload;
    }
}

public sealed class RoomPushHub
{
    private readonly ConcurrentDictionary<int, Action<RoomPushHead>> _senders = new();
    private long _sentCount;
    private long _droppedCount;

    public long SentCount => Interlocked.Read(ref _sentCount);
    public long DroppedCount => Interlocked.Read(ref _droppedCount);

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
            Interlocked.Increment(ref _droppedCount);
            return;
        }

        NetworkBuffer payloadWriter = NetworkBufferPool.Shared.Get();
        MemoryPackSerializer.Serialize(payloadWriter, push);
        var head = new RoomPushHead(TypeId<TPush>.stableId16, payloadWriter.ToArraySegment());
        sender(head);
        Interlocked.Increment(ref _sentCount);
        NetworkBufferPool.Shared.Return(payloadWriter);
    }

    public void SendMany<TPush>(IEnumerable<int> connectionIds, TPush push)
        where TPush : IRoomPush
    {
        NetworkBuffer payloadWriter = NetworkBufferPool.Shared.Get();
        MemoryPackSerializer.Serialize(payloadWriter, push);
        var head = new RoomPushHead(TypeId<TPush>.stableId16, payloadWriter.ToArraySegment());

        foreach (int connectionId in connectionIds)
        {
            if (_senders.TryGetValue(connectionId, out Action<RoomPushHead>? sender))
            {
                sender(head);
                Interlocked.Increment(ref _sentCount);
            }
            else
            {
                Interlocked.Increment(ref _droppedCount);
            }
        }

        NetworkBufferPool.Shared.Return(payloadWriter);
    }
}

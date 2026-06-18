using System.Collections.Concurrent;

namespace GameServer.Core.Rooms;

public sealed class RoomConnectionRegistry
{
    private readonly ConcurrentDictionary<int, RoomConnectionContext> _connections = new();
    private int _nextConnectionId;

    public int Add(long uid, string roomId)
    {
        int connectionId = Interlocked.Increment(ref _nextConnectionId);
        _connections[connectionId] = new RoomConnectionContext(uid, roomId);
        return connectionId;
    }

    public bool TryGet(int connectionId, out RoomConnectionContext context)
    {
        return _connections.TryGetValue(connectionId, out context);
    }

    public bool TrySetRoom(int connectionId, string roomId)
    {
        while (_connections.TryGetValue(connectionId, out RoomConnectionContext context))
        {
            RoomConnectionContext updated = context with { RoomId = roomId };
            if (_connections.TryUpdate(connectionId, updated, context))
            {
                return true;
            }
        }

        return false;
    }

    public List<int> GetRoomConnectionIds(string roomId)
    {
        var result = new List<int>();
        foreach (KeyValuePair<int, RoomConnectionContext> item in _connections)
        {
            if (item.Value.RoomId == roomId)
            {
                result.Add(item.Key);
            }
        }

        return result;
    }

    public void Remove(int connectionId)
    {
        // Push senders are owned by RoomWorkerBase.RemoveConnectionAsync for direct transports.
        _connections.TryRemove(connectionId, out _);
    }

    public bool TryRemove(int connectionId, out RoomConnectionContext context)
    {
        return _connections.TryRemove(connectionId, out context);
    }
}

public readonly record struct RoomConnectionContext(long Uid, string RoomId);

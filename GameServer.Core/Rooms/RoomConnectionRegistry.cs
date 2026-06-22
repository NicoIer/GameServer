using System.Collections.Concurrent;

namespace GameServer.Core.Rooms;

public sealed class RoomConnectionRegistry
{
    private readonly ConcurrentDictionary<int, RoomConnectionContext> _connections = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, byte>> _roomConnections = new(StringComparer.Ordinal);
    private int _nextConnectionId;

    public int Add(long uid, string roomId)
    {
        int connectionId = Interlocked.Increment(ref _nextConnectionId);
        _connections[connectionId] = new RoomConnectionContext(uid, roomId);
        AddRoomConnection(roomId, connectionId);
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
            if (string.Equals(context.RoomId, roomId, StringComparison.Ordinal))
            {
                return true;
            }

            RoomConnectionContext updated = context with { RoomId = roomId };
            if (_connections.TryUpdate(connectionId, updated, context))
            {
                RemoveRoomConnection(context.RoomId, connectionId);
                AddRoomConnection(roomId, connectionId);
                return true;
            }
        }

        return false;
    }

    public List<int> GetRoomConnectionIds(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return new List<int>();
        }

        if (!_roomConnections.TryGetValue(roomId, out ConcurrentDictionary<int, byte>? connections))
        {
            return new List<int>();
        }

        return new List<int>(connections.Keys);
    }

    public void Remove(int connectionId)
    {
        // Push senders are owned by RoomWorkerBase.RemoveConnectionAsync for direct transports.
        if (_connections.TryRemove(connectionId, out RoomConnectionContext context))
        {
            RemoveRoomConnection(context.RoomId, connectionId);
        }
    }

    public bool TryRemove(int connectionId, out RoomConnectionContext context)
    {
        if (!_connections.TryRemove(connectionId, out context))
        {
            return false;
        }

        RemoveRoomConnection(context.RoomId, connectionId);
        return true;
    }

    private void AddRoomConnection(string roomId, int connectionId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return;
        }

        ConcurrentDictionary<int, byte> connections = _roomConnections.GetOrAdd(
            roomId,
            _ => new ConcurrentDictionary<int, byte>());
        connections[connectionId] = 0;
    }

    private void RemoveRoomConnection(string roomId, int connectionId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return;
        }

        if (!_roomConnections.TryGetValue(roomId, out ConcurrentDictionary<int, byte>? connections))
        {
            return;
        }

        connections.TryRemove(connectionId, out _);
    }
}

public readonly record struct RoomConnectionContext(long Uid, string RoomId);

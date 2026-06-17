namespace GameServer.Core.Rooms;

public sealed class RoomConnectionRegistry
{
    private readonly Dictionary<int, RoomConnectionContext> _connections = new();
    private readonly object _lock = new();
    private int _nextConnectionId;

    public int Add(long uid, string roomId)
    {
        lock (_lock)
        {
            int connectionId = ++_nextConnectionId;
            _connections[connectionId] = new RoomConnectionContext(uid, roomId);
            return connectionId;
        }
    }

    public bool TryGet(int connectionId, out RoomConnectionContext context)
    {
        lock (_lock)
        {
            return _connections.TryGetValue(connectionId, out context);
        }
    }

    public bool TrySetRoom(int connectionId, string roomId)
    {
        lock (_lock)
        {
            if (!_connections.TryGetValue(connectionId, out RoomConnectionContext context))
            {
                return false;
            }

            _connections[connectionId] = context with { RoomId = roomId };
            return true;
        }
    }

    public void Remove(int connectionId)
    {
        lock (_lock)
        {
            _connections.Remove(connectionId);
        }
    }
}

public readonly record struct RoomConnectionContext(long Uid, string RoomId);

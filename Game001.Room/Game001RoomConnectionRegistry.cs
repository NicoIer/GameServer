namespace Game001.Room;

public sealed class Game001RoomConnectionRegistry
{
    private readonly Dictionary<int, Game001RoomConnectionContext> _connections = new();
    private readonly object _lock = new();
    private int _nextConnectionId;

    public int Add(long uid, string roomId)
    {
        lock (_lock)
        {
            int connectionId = ++_nextConnectionId;
            _connections[connectionId] = new Game001RoomConnectionContext(uid, roomId);
            return connectionId;
        }
    }

    public bool TryGet(int connectionId, out Game001RoomConnectionContext context)
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
            if (!_connections.TryGetValue(connectionId, out Game001RoomConnectionContext context))
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

public readonly record struct Game001RoomConnectionContext(long Uid, string RoomId);

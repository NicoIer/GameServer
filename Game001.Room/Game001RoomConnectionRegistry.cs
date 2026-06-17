namespace Game001.Room;

public sealed class Game001RoomConnectionRegistry
{
    private readonly Dictionary<int, Game001RoomConnectionContext> _connections = new();
    private int _nextConnectionId;

    public int Add(long uid, string roomId)
    {
        int connectionId = ++_nextConnectionId;
        _connections[connectionId] = new Game001RoomConnectionContext(uid, roomId);
        return connectionId;
    }

    public bool TryGet(int connectionId, out Game001RoomConnectionContext context)
    {
        return _connections.TryGetValue(connectionId, out context);
    }

    public void Remove(int connectionId)
    {
        _connections.Remove(connectionId);
    }
}

public readonly record struct Game001RoomConnectionContext(long Uid, string RoomId);

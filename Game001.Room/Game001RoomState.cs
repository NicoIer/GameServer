namespace Game001.Room;

public sealed class Game001RoomState
{
    private readonly Dictionary<string, HashSet<long>> _rooms = new();
    private readonly object _lock = new();

    public string CreateRoom(long uid, string roomId)
    {
        lock (_lock)
        {
            if (!_rooms.TryGetValue(roomId, out HashSet<long>? players))
            {
                players = new HashSet<long>();
                _rooms[roomId] = players;
            }

            players.Add(uid);
            return $"created room={roomId} players={players.Count}";
        }
    }

    public string JoinRoom(long uid, string roomId)
    {
        lock (_lock)
        {
            if (!_rooms.TryGetValue(roomId, out HashSet<long>? players))
            {
                return string.Empty;
            }

            players.Add(uid);
            return $"joined room={roomId} players={players.Count}";
        }
    }

    public string PingRoom(long uid, string roomId)
    {
        lock (_lock)
        {
            int count = 0;
            if (_rooms.TryGetValue(roomId, out HashSet<long>? players))
            {
                count = players.Count;
            }

            return $"pong uid={uid} room={roomId} players={count}";
        }
    }
}

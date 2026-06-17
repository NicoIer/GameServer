namespace Game001.Room;

public sealed class Game001RoomState
{
    private readonly Dictionary<string, HashSet<long>> _rooms = new();

    public RoomStateResult CreateRoom(long uid, string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out HashSet<long>? players))
        {
            players = new HashSet<long>();
            _rooms[roomId] = players;
        }

        players.Add(uid);
        return new RoomStateResult(true, $"created room={roomId} players={players.Count}", players.Count);
    }

    public RoomStateResult JoinRoom(long uid, string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out HashSet<long>? players))
        {
            return new RoomStateResult(false, $"room not found room={roomId}", 0);
        }

        players.Add(uid);
        return new RoomStateResult(true, $"joined room={roomId} players={players.Count}", players.Count);
    }

    public RoomStateResult LeaveRoom(long uid, string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out HashSet<long>? players))
        {
            return new RoomStateResult(false, $"room not found room={roomId}", 0);
        }

        players.Remove(uid);
        return new RoomStateResult(true, $"left room={roomId} players={players.Count}", players.Count);
    }

    public RoomStateResult PingRoom(long uid, string roomId)
    {
        if (!_rooms.TryGetValue(roomId, out HashSet<long>? players))
        {
            return new RoomStateResult(false, $"room not found room={roomId}", 0);
        }

        return new RoomStateResult(true, $"pong uid={uid} room={roomId} players={players.Count}", players.Count);
    }
}

public readonly record struct RoomStateResult(bool Success, string Message, int PlayerCount);

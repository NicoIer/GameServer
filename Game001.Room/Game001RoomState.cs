namespace Game001.Room;

public readonly record struct RoomStateResult(bool Success, string Message, int PlayerCount);

public sealed class RoomRuntimeState
{
    public string RoomId { get; }
    public HashSet<long> Players { get; } = new();
    public int Frame { get; private set; }
    public long LastUpdateTimeMs { get; private set; }

    public RoomRuntimeState(string roomId)
    {
        RoomId = roomId;
    }

    public RoomStateResult CreateRoom(long uid)
    {
        Players.Add(uid);
        return new RoomStateResult(true, $"created room={RoomId} players={Players.Count}", Players.Count);
    }

    public RoomStateResult JoinRoom(long uid)
    {
        Players.Add(uid);
        return new RoomStateResult(true, $"joined room={RoomId} players={Players.Count}", Players.Count);
    }

    public RoomStateResult LeaveRoom(long uid)
    {
        Players.Remove(uid);
        return new RoomStateResult(true, $"left room={RoomId} players={Players.Count}", Players.Count);
    }

    public RoomStateResult PingRoom(long uid)
    {
        return new RoomStateResult(true, $"pong uid={uid} room={RoomId} players={Players.Count}", Players.Count);
    }

    public void Update(long timeNowMs, int frame)
    {
        Frame = frame;
        LastUpdateTimeMs = timeNowMs;
    }
}

using Game001.Core;

namespace Game001.Room.Runtime;

public sealed class Game001RoomState
{
    public string RoomId { get; }
    public HashSet<long> Players { get; } = new();
    public HashSet<long> DisconnectedPlayers { get; } = new();
    public int Frame { get; private set; }
    public long LastUpdateTimeMs { get; private set; }

    public Game001RoomState(string roomId)
    {
        RoomId = roomId;
    }

    public void SetFrame(long timeNowMs, int frame)
    {
        Frame = frame;
        LastUpdateTimeMs = timeNowMs;
    }

    public RoomInfo CreateRoomInfo(long serverTimeMs)
    {
        return new RoomInfo
        {
            RoomId = RoomId,
            PlayerCount = Players.Count,
            Frame = Frame,
            ServerTimeMs = serverTimeMs,
        };
    }
}

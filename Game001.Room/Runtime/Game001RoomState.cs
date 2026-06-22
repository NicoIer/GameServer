using Game001.Core;
using GameServer.Core.Rooms;

namespace Game001.Room.Runtime;

public sealed class Game001RoomState
{
    private int _lifecycleState = (int)RoomLifecycleState.Created;

    public string RoomId { get; }
    public HashSet<long> Players { get; } = new();
    public HashSet<long> DisconnectedPlayers { get; } = new();
    public Dictionary<long, long> DisconnectedPlayerTimesMs { get; } = new();
    public RoomLifecycleState LifecycleState => (RoomLifecycleState)Volatile.Read(ref _lifecycleState);
    public long EmptySinceTimeMs { get; private set; }
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

    public void SetActive(long timeNowMs)
    {
        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Active);
        EmptySinceTimeMs = 0;
    }

    public void SetEmpty(long timeNowMs)
    {
        if (LifecycleState == RoomLifecycleState.Empty)
        {
            return;
        }

        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Empty);
        EmptySinceTimeMs = timeNowMs;
    }

    public void SetClosing(long timeNowMs)
    {
        if (LifecycleState == RoomLifecycleState.Closed)
        {
            return;
        }

        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Closing);
    }

    public void SetClosed(long timeNowMs)
    {
        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Closed);
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

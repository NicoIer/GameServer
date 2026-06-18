using Game001.Core;
using Game001.Room.Runtime;
using GameServer.Core.Rooms;

namespace Game001.Room.Systems;

public sealed class RoomSyncSystem
{
    private readonly RoomPushHub _pushHub;
    private readonly Game001RoomState _state;

    public RoomSyncSystem(RoomPushHub pushHub, Game001RoomState state)
    {
        _pushHub = pushHub;
        _state = state;
    }

    public void SendFullState(int connectionId)
    {
        var push = new RoomFullStatePush
        {
            Room = _state.CreateRoomInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            Players = _state.Players.ToArray(),
            DisconnectedPlayers = _state.DisconnectedPlayers.ToArray(),
        };

        _pushHub.Send(connectionId, push);
    }

    // public void MarkDirty()
    // {
    // }

    public void Update(long timeNowMs, int frame)
    {
    }
}

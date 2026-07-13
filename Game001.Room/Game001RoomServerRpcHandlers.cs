using Game001.Core.Generated;
using Game001.Room.Runtime;
using GameServer.Core.Rooms;

namespace Game001.Room;

public sealed partial class Game001RoomServerRpcHandlers : IGame001ServerRpcHandlers
{
    private readonly RoomConnectionRegistry _connections;
    private readonly Game001RoomState _state;

    public Game001RoomServerRpcHandlers(
        RoomConnectionRegistry connections,
        Game001RoomState state)
    {
        _connections = connections;
        _state = state;
    }
}

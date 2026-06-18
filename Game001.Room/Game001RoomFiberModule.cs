using Game001.Core.Generated;
using GameServer.Core.Rooms;
using Network;

namespace Game001.Room;

public sealed class Game001RoomFiberModule : RoomFiberModuleBase
{
    private readonly RoomConnectionRegistry _connections;
    private readonly RoomRuntimeState _state;

    public Game001RoomFiberModule(string roomId, RoomConnectionRegistry connections, int roomFrameRate)
        : base(roomId, roomFrameRate)
    {
        _connections = connections;
        _state = new RoomRuntimeState(roomId);
    }

    protected override void RegisterHandlers(ReqRspServerCenter center)
    {
        var handlers = new Game001RoomReqRspHandlers(_connections, _state, FrameAwaiter);
        NetworkReqRspInitializer.RegisterAll(center, handlers);
    }

    protected override void OnRoomUpdate(long timeNowMs, int frame)
    {
        _state.Update(timeNowMs, frame);
    }

    public override ValueTask HandleConnectionDisconnectedAsync(int connectionId, RoomConnectionContext context)
    {
        _state.DisconnectRoom(context.Uid);
        return ValueTask.CompletedTask;
    }
}

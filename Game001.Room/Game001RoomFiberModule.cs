using Game001.Core.Generated;
using Game001.Room.Runtime;
using GameServer.Core.Rooms;
using Network;

namespace Game001.Room;

public sealed class Game001RoomFiberModule : RoomFiberModuleBase
{
    private readonly RoomConnectionRegistry _connections;
    private readonly Game001Room _room;

    public Game001RoomFiberModule(string roomId, RoomConnectionRegistry connections, RoomPushHub pushHub, int roomFrameRate)
        : base(roomId, roomFrameRate)
    {
        _connections = connections;
        _room = new Game001Room(roomId, pushHub);
    }

    public override RoomLifecycleState LifecycleState => _room.LifecycleState;
    public override int PlayerCount => _room.State.PlayerCount;

    protected override void RegisterHandlers(ReqRspServerCenter center)
    {
        var handlers = new Game001RoomReqRspHandlers(_connections, _room, FrameAwaiter);
        NetworkReqRspInitializer.RegisterAll(center, handlers);
    }

    protected override void OnRoomUpdate(long timeNowMs, int frame)
    {
        _room.Update(timeNowMs, frame);
    }

    public override ValueTask HandleConnectionDisconnectedAsync(int connectionId, RoomConnectionContext context)
    {
        _room.DisconnectRoom(connectionId, context.Uid);
        return ValueTask.CompletedTask;
    }

    public override bool ShouldCloseRoom(long timeNowMs)
    {
        return _room.ShouldCloseRoom(timeNowMs);
    }

    public override void BeginCloseRoom(long timeNowMs)
    {
        _room.BeginCloseRoom(timeNowMs);
    }

    protected override void OnRoomStopped()
    {
        _room.CloseRoom(Environment.TickCount64);
    }
}

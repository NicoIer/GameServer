using Game001.Core.Generated;
using Game001.Room.Runtime;
using Game001.Room.Systems;
using GameServer.Core.Rooms;
using Network;

namespace Game001.Room;

public sealed class Game001RoomFiberModule : RoomFiberModuleBase
{
    private readonly RoomConnectionRegistry _connections;
    private readonly RoomLifecycleSystem _lifecycleSystem;
    private readonly Game001Room _room;

    public Game001RoomFiberModule(string roomId, RoomConnectionRegistry connections, RoomPushHub pushHub, int roomFrameRate)
        : base(roomId, roomFrameRate)
    {
        _connections = connections;
        _room = new Game001Room(roomId, pushHub);
        if (!_room.Systems.TryGetSystem(out RoomLifecycleSystem? lifecycleSystem))
        {
            throw new InvalidOperationException("missing RoomLifecycleSystem");
        }

        _lifecycleSystem = lifecycleSystem;
    }

    public override RoomLifecycleState LifecycleState => _room.LifecycleState;
    public override int PlayerCount => _room.State.PlayerCount;

    protected override void RegisterHandlers(ReqRspServerCenter center)
    {
        var handlers = new Game001RoomReqRspHandlers(_connections, _lifecycleSystem);
        NetworkReqRspInitializer.RegisterAll(center, handlers);
    }

    protected override void OnRoomUpdate(long timeNowMs, int frame)
    {
        _room.Update(timeNowMs, frame);
    }

    public override ValueTask HandleConnectionDisconnectedAsync(int connectionId, RoomConnectionContext context)
    {
        _lifecycleSystem.DisconnectRoom(connectionId, context.Uid);
        return ValueTask.CompletedTask;
    }

    public override bool ShouldCloseRoom(long timeNowMs)
    {
        return _lifecycleSystem.ShouldCloseRoom(timeNowMs);
    }

    public override void BeginCloseRoom(long timeNowMs)
    {
        _lifecycleSystem.BeginCloseRoom(timeNowMs);
    }

    protected override void OnRoomStopped()
    {
        _lifecycleSystem.CloseRoom(Environment.TickCount64);
        _room.Destroy();
    }
}

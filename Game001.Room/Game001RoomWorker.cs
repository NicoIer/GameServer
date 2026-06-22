using Game001.Core;
using GameServer.Core.Rooms;

namespace Game001.Room;

public sealed class Game001RoomWorker : RoomWorkerBase<Game001RoomFiberModule>
{
    private const string DefaultRoomId = "room-001";

    private readonly RoomRequestRouter _requestRouter = CreateRequestRouter();

    public Game001RoomWorker(RoomConnectionRegistry connections, RoomPushHub pushHub, int roomFrameRate)
        : base(connections, pushHub, roomFrameRate)
    {
    }

    protected override RoomRequestRouter RequestRouter => _requestRouter;

    protected override Game001RoomFiberModule CreateRoomModule(string roomId)
    {
        return new Game001RoomFiberModule(roomId, Connections, PushHub, RoomFrameRate);
    }

    protected override string CreateRoomFiberName(string roomId)
    {
        return $"Game001.RoomRoot.{roomId}";
    }

    private static RoomRequestRouter CreateRequestRouter()
    {
        var router = new RoomRequestRouter();
        router.Register<CreateRoomReq>(
            (req, context) => ResolveRoomId(req.RoomId, context.RoomId),
            canCreateRoom: true,
            successConnectionAction: RoomRequestConnectionAction.BindRoom);
        router.Register<JoinRoomReq>(
            (req, context) => ResolveConnectedRoomId(req.RoomId, context.RoomId),
            canCreateRoom: false,
            successConnectionAction: RoomRequestConnectionAction.BindRoom);
        router.Register<LeaveRoomReq>(
            (req, context) => ResolveConnectedRoomId(req.RoomId, context.RoomId),
            canCreateRoom: false,
            successConnectionAction: RoomRequestConnectionAction.ClearRoom);
        router.Register<RoomPingReq>(
            (req, context) => ResolveConnectedRoomId(req.RoomId, context.RoomId),
            canCreateRoom: false,
            successConnectionAction: RoomRequestConnectionAction.None);
        return router;
    }

    private static string ResolveRoomId(string? messageRoomId, string contextRoomId)
    {
        if (!string.IsNullOrWhiteSpace(messageRoomId))
        {
            return messageRoomId;
        }

        if (!string.IsNullOrWhiteSpace(contextRoomId))
        {
            return contextRoomId;
        }

        return DefaultRoomId;
    }

    private static string ResolveConnectedRoomId(string? messageRoomId, string contextRoomId)
    {
        if (!string.IsNullOrWhiteSpace(messageRoomId))
        {
            return messageRoomId;
        }

        if (!string.IsNullOrWhiteSpace(contextRoomId))
        {
            return contextRoomId;
        }

        return string.Empty;
    }
}

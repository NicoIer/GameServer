using Game001.Core;
using GameServer.Core.Rooms;

namespace Game001.Room;

public sealed class Game001RoomWorker : RoomWorkerBase<Game001RoomFiberModule>
{
    private const string DefaultRoomId = "room-001";

    private readonly RoomRequestRouter _requestRouter;

    public Game001RoomWorker(RoomConnectionRegistry connections, RoomPushHub pushHub, int roomFrameRate, string workerId)
        : base(connections, pushHub, roomFrameRate, workerId)
    {
        _requestRouter = CreateRequestRouter();
    }

    protected override RoomRequestRouter RequestRouter => _requestRouter;

    protected override Game001RoomFiberModule CreateRoomModule(string roomId)
    {
        return new Game001RoomFiberModule(roomId, Connections, PushHub, RoomFrameRate, WorkerId);
    }

    protected override string CreateRoomFiberName(string roomId)
    {
        return $"Game001.RoomRoot.{roomId}";
    }

    private RoomRequestRouter CreateRequestRouter()
    {
        var router = new RoomRequestRouter();
        router.RegisterWorker<ListRoomsReq, ListRoomsRsp>(HandleListRooms);
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

    private ValueTask<(ListRoomsRsp rsp, Network.ErrorCode errorCode, string errorMsg)> HandleListRooms(
        int connectionId,
        ListRoomsReq req,
        RoomConnectionContext context)
    {
        List<RoomMetrics> metrics = GetRoomMetrics();
        var rooms = new RoomListItem[metrics.Count];
        for (int i = 0; i < metrics.Count; i++)
        {
            RoomMetrics metric = metrics[i];
            rooms[i] = new RoomListItem
            {
                RoomId = metric.RoomId,
                PlayerCount = metric.PlayerCount,
                ConnectionCount = metric.ConnectionCount,
                LifecycleState = metric.LifecycleState,
            };
        }

        var rsp = new ListRoomsRsp
        {
            Rooms = new ArraySegment<RoomListItem>(rooms),
        };
        return new ValueTask<(ListRoomsRsp, Network.ErrorCode, string)>((rsp, Network.ErrorCode.Success, $"rooms={rooms.Length}"));
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

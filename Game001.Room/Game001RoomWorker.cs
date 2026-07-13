using Game001.Core;
using GameServer.Core.Rooms;

namespace Game001.Room;

public sealed partial class Game001RoomWorker : RoomWorkerBase<Game001RoomFiberModule>
{
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

}

using Network;

namespace GameServer.Core.Rooms;

public interface IRoomWorker
{
    string WorkerId { get; }
    RoomPushHub PushHub { get; }
    int RoomCount { get; }
    int ClosingRoomCount { get; }
    int OnlineConnectionCount { get; }
    Task<int> AddConnectionAsync(long uid, string roomId);
    Task RemoveConnectionAsync(int connectionId);
    Task<RspHead> HandleRequestAsync(int connectionId, ReqHead request, NetworkBuffer responsePayloadWriter);
    void HandleCommand(int connectionId, RoomCommandHead command);
    void Update(long timeNowMs);
    void Stop();
    RoomWorkerMetrics GetMetrics();
    List<RoomMetrics> GetRoomMetrics();
}

using Network;

namespace GameServer.Core.Rooms;

public interface IRoomWorker
{
    Task<int> AddConnectionAsync(long uid, string roomId);
    Task RemoveConnectionAsync(int connectionId);
    Task<RspHead> HandleRequestAsync(int connectionId, ReqHead request);
}

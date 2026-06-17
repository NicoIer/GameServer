using GameServer.Core.Protocol;
using Grpc.Core;

namespace Game001.Room;

public sealed class Game001RoomServiceImpl : GameIngress.GameIngressBase
{
    private readonly Game001RoomWorker _worker;

    public Game001RoomServiceImpl(Game001RoomWorker worker)
    {
        _worker = worker;
    }

    public override async Task<GameResponse> Handle(GameRequest request, ServerCallContext context)
    {
        return await _worker.HandleDataAsync(request.Uid, request.Data);
    }
}

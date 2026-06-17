using GameServer.Core.Protocol;
using Grpc.Core;

namespace Game001.Room;

public sealed class Game001RoomServiceImpl : GameIngress.GameIngressBase
{
    private readonly Game001RoomReqRspDispatcher _dispatcher;

    public Game001RoomServiceImpl(Game001RoomReqRspDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override Task<GameResponse> Handle(GameRequest request, ServerCallContext context)
    {
        GameResponse response = _dispatcher.HandleData(request.Uid, request.Data);
        return Task.FromResult(response);
    }
}

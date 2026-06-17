using GameServer.Core.Protocol;
using Grpc.Core;

namespace Game001.Room;

public sealed class Game001RoomServiceImpl : GameIngress.GameIngressBase
{
    private readonly Game001RoomPacketHandler _handler;

    public Game001RoomServiceImpl(Game001RoomPacketHandler handler)
    {
        _handler = handler;
    }

    public override Task<GameResponse> Handle(GameRequest request, ServerCallContext context)
    {
        GameResponse response = _handler.HandleData(request.Uid, request.RouteId, request.Data);
        return Task.FromResult(response);
    }
}

using Grpc.Core;
using GameServer.Rpc.Server;

namespace GameServer.Rpc.Services;

public sealed class GrpcTestService : GrpcTest.GrpcTestBase
{
    private readonly GrpcMessageDispatcher _dispatcher;

    public GrpcTestService(GrpcMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override Task<GrpcTestReply> Ping(GrpcTestRequest request, ServerCallContext context)
    {
        return _dispatcher.SendAsync<GrpcTestRequest, GrpcTestReply>(
            request,
            context.CancellationToken);
    }
}

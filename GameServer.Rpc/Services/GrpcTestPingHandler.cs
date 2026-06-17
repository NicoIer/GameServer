using GameServer.Rpc.Server;

namespace GameServer.Rpc.Services;

[GrpcMessageHandler(typeof(GrpcTestRequest))]
public sealed class GrpcTestPingHandler : GrpcMessageHandler<GrpcTestRequest, GrpcTestReply>
{
    protected override GrpcTestReply Run(GrpcTestRequest request)
    {
        return new GrpcTestReply
        {
            Message = request.Message,
            ServerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };
    }
}

using GameServer.Center.Login;
using GameServer.Core.Protocol;
using Grpc.Core;

namespace GameServer.Center;

public sealed class CenterServiceImpl : CenterService.CenterServiceBase
{
    private readonly LoginProviderRegistry _loginProviders;
    private readonly CenterRegistry _registry;

    public CenterServiceImpl(CenterRegistry registry, LoginProviderRegistry loginProviders)
    {
        _registry = registry;
        _loginProviders = loginProviders;
    }

    public override Task<AuthReply> Auth(AuthRequest request, ServerCallContext context)
    {
        LoginResult loginResult = _loginProviders.Login(request);
        if (loginResult.Error != ErrorCode.Success)
        {
            return Task.FromResult(new AuthReply { Error = loginResult.Error });
        }

        string token = _registry.CreateToken(loginResult.Uid);
        return Task.FromResult(new AuthReply
        {
            Error = ErrorCode.Success,
            Token = token,
            Uid = loginResult.Uid,
        });
    }

    public override Task<ValidateTokenReply> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        long uid = _registry.ValidateToken(request.Token);
        if (uid == 0)
        {
            return Task.FromResult(new ValidateTokenReply { Error = ErrorCode.Unauthorized });
        }

        return Task.FromResult(new ValidateTokenReply
        {
            Error = ErrorCode.Success,
            Uid = uid,
        });
    }

    public override Task<RegisterServiceReply> RegisterService(RegisterServiceRequest request, ServerCallContext context)
    {
        if (request.Endpoint == null ||
            request.Endpoint.GameId.Length == 0 ||
            request.Endpoint.Target.Length == 0 ||
            request.Endpoint.Address.Length == 0)
        {
            return Task.FromResult(new RegisterServiceReply { Error = ErrorCode.InvalidRequest });
        }

        _registry.Register(request.Endpoint);
        return Task.FromResult(new RegisterServiceReply { Error = ErrorCode.Success });
    }

    public override Task<ResolveServiceReply> ResolveService(ResolveServiceRequest request, ServerCallContext context)
    {
        if (request.GameId.Length == 0 || request.Target.Length == 0)
        {
            return Task.FromResult(new ResolveServiceReply { Error = ErrorCode.InvalidRequest });
        }

        ServiceEndpoint? endpoint = _registry.Resolve(request.GameId, request.Target, request.RouteId);
        if (endpoint == null)
        {
            return Task.FromResult(new ResolveServiceReply { Error = ErrorCode.RouteNotFound });
        }

        return Task.FromResult(new ResolveServiceReply
        {
            Error = ErrorCode.Success,
            Endpoint = endpoint,
        });
    }

    public override Task<ListServiceEndpointsReply> ListServiceEndpoints(ListServiceEndpointsRequest request, ServerCallContext context)
    {
        var reply = new ListServiceEndpointsReply
        {
            Error = ErrorCode.Success,
        };
        reply.Endpoints.AddRange(_registry.ListServices(request.Target));
        return Task.FromResult(reply);
    }
}

using GameServer.Core.Protocol;
using Grpc.Core;

namespace GameServer.Gate;

public sealed class GateServiceImpl : GateService.GateServiceBase
{
    private readonly CenterService.CenterServiceClient _centerClient;
    private readonly GameIngressClientCache _gameClients;

    public GateServiceImpl(CenterService.CenterServiceClient centerClient, GameIngressClientCache gameClients)
    {
        _centerClient = centerClient;
        _gameClients = gameClients;
    }

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        AuthReply authReply = await _centerClient.AuthAsync(new AuthRequest
        {
            LoginType = request.LoginType,
            Credential = request.Credential,
            DeviceId = request.DeviceId,
            ClientVersion = request.ClientVersion,
        });

        return new LoginReply
        {
            Error = authReply.Error,
            Token = authReply.Token,
            Uid = authReply.Uid,
        };
    }

    public override async Task<ForwardReply> Forward(ForwardRequest request, ServerCallContext context)
    {
        if (request.Envelope == null)
        {
            return new ForwardReply { Error = ErrorCode.InvalidRequest };
        }

        ValidateTokenReply validateReply = await _centerClient.ValidateTokenAsync(new ValidateTokenRequest
        {
            Token = request.Token,
        });

        if (validateReply.Error != ErrorCode.Success)
        {
            return new ForwardReply { Error = ErrorCode.Unauthorized };
        }

        ClientEnvelope envelope = request.Envelope;
        if (envelope.GameId.Length == 0 || envelope.Target.Length == 0)
        {
            return new ForwardReply { Error = ErrorCode.InvalidRequest };
        }

        ResolveServiceReply resolveReply = await _centerClient.ResolveServiceAsync(new ResolveServiceRequest
        {
            GameId = envelope.GameId,
            Target = envelope.Target,
            RouteId = envelope.RouteId,
        });

        if (resolveReply.Error != ErrorCode.Success)
        {
            return new ForwardReply { Error = ErrorCode.RouteNotFound };
        }

        GameIngress.GameIngressClient gameClient = _gameClients.GetClient(resolveReply.Endpoint.Address);
        GameResponse gameResponse = await gameClient.HandleAsync(new GameRequest
        {
            Uid = validateReply.Uid,
            SessionId = request.Token,
            GameId = envelope.GameId,
            Target = envelope.Target,
            RouteId = envelope.RouteId,
            Opcode = envelope.Opcode,
            Payload = envelope.Payload,
        });

        return new ForwardReply
        {
            Error = gameResponse.Error,
            Opcode = gameResponse.Opcode,
            Payload = gameResponse.Payload,
        };
    }
}

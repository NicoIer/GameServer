using GameServer.Core.Protocol;
using Grpc.Core;
using ProtocolGameType = GameServer.Core.Protocol.GameType;

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

    public override async Task<ListGameWorkersReply> ListGameWorkers(ListGameWorkersRequest request, ServerCallContext context)
    {
        ValidateTokenReply validateReply = await _centerClient.ValidateTokenAsync(new ValidateTokenRequest
        {
            Token = request.Token,
        });

        if (validateReply.Error != ErrorCode.Success)
        {
            return new ListGameWorkersReply { Error = ErrorCode.Unauthorized };
        }

        ListServiceEndpointsReply endpointsReply = await _centerClient.ListServiceEndpointsAsync(new ListServiceEndpointsRequest
        {
            Target = request.Target,
        });

        var reply = new ListGameWorkersReply
        {
            Error = endpointsReply.Error,
        };
        if (endpointsReply.Error != ErrorCode.Success)
        {
            return reply;
        }

        foreach (ServiceEndpoint endpoint in endpointsReply.Endpoints)
        {
            reply.Workers.Add(new GameWorkerInfo
            {
                GameType = endpoint.GameType,
                Target = endpoint.Target,
                RouteId = endpoint.RouteId,
            });
        }

        return reply;
    }

    public override async Task<PrepareRoomConnectionReply> PrepareRoomConnection(PrepareRoomConnectionRequest request, ServerCallContext context)
    {
        ValidateTokenReply validateReply = await _centerClient.ValidateTokenAsync(new ValidateTokenRequest
        {
            Token = request.Token,
        });

        if (validateReply.Error != ErrorCode.Success)
        {
            return new PrepareRoomConnectionReply { Error = ErrorCode.Unauthorized };
        }

        if (request.GameType == ProtocolGameType.Unspecified || request.Target.Length == 0)
        {
            return new PrepareRoomConnectionReply { Error = ErrorCode.InvalidRequest };
        }

        ResolveServiceReply resolveReply = await _centerClient.ResolveServiceAsync(new ResolveServiceRequest
        {
            GameType = request.GameType,
            Target = request.Target,
            RouteId = request.RouteId,
        });

        if (resolveReply.Error != ErrorCode.Success ||
            resolveReply.Endpoint.DirectProtocol == DirectTransportProtocol.Unspecified ||
            resolveReply.Endpoint.DirectAddress.Length == 0)
        {
            return new PrepareRoomConnectionReply { Error = ErrorCode.RouteNotFound };
        }

        if (!TryParseAddress(resolveReply.Endpoint.DirectAddress, out string host, out int port))
        {
            return new PrepareRoomConnectionReply { Error = ErrorCode.InvalidRequest };
        }

        return new PrepareRoomConnectionReply
        {
            Error = ErrorCode.Success,
            GameType = resolveReply.Endpoint.GameType,
            Target = resolveReply.Endpoint.Target,
            RouteId = resolveReply.Endpoint.RouteId,
            DirectProtocol = resolveReply.Endpoint.DirectProtocol,
            Host = host,
            Port = port,
            ConnectTicket = request.Token,
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
        if (envelope.GameType == ProtocolGameType.Unspecified || envelope.Target.Length == 0)
        {
            return new ForwardReply { Error = ErrorCode.InvalidRequest };
        }

        ResolveServiceReply resolveReply = await _centerClient.ResolveServiceAsync(new ResolveServiceRequest
        {
            GameType = envelope.GameType,
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
            GameType = envelope.GameType,
            Target = envelope.Target,
            RouteId = envelope.RouteId,
            Data = envelope.Data,
        });

        return new ForwardReply
        {
            Error = gameResponse.Error,
            Data = gameResponse.Data,
        };
    }

    private static bool TryParseAddress(string address, out string host, out int port)
    {
        int splitIndex = address.LastIndexOf(':');
        if (splitIndex <= 0 || splitIndex == address.Length - 1)
        {
            host = string.Empty;
            port = 0;
            return false;
        }

        host = address[..splitIndex];
        return int.TryParse(address[(splitIndex + 1)..], out port);
    }
}

using Game001.ClientTestApp;
using Game001.Core;
using GameServer.Core.Grpc;
using GameServer.Core.Network;
using GameServer.Core.Protocol;
using Grpc.Net.Client;
using MemoryPack;
using Network;
using UnityToolkit;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

const string GameId = "Game001";
const string TargetRoomWorker = "room-worker";
const string DefaultWorkerId = "worker-001";
const string DefaultRoomId = "room-001";
const ushort UnknownReqHash = 999;

string gateAddress = ReadOption(args, "--gate-address", "GATE_ADDRESS", "http://127.0.0.1:5002");
string loginType = ReadOption(args, "--login-type", "CLIENT_TEST_LOGIN_TYPE", "guest");
string credential = ReadOption(args, "--credential", "CLIENT_TEST_CREDENTIAL", "guest-client-test");
string deviceId = ReadOption(args, "--device-id", "CLIENT_TEST_DEVICE_ID", credential);
string clientVersion = ReadOption(args, "--client-version", "CLIENT_TEST_VERSION", "dev");
ushort requestIndex = 0;

using GrpcChannel gateChannel = GrpcClientFactory.CreateChannel(gateAddress);
var gateClient = new GateService.GateServiceClient(gateChannel);

Console.WriteLine($"Gate: {gateAddress}");
Console.WriteLine($"LoginType: {loginType}");
Console.WriteLine($"Credential: {credential}");
Console.WriteLine($"DeviceId: {deviceId}");

LoginReply loginReply = await gateClient.LoginAsync(new LoginRequest
{
    LoginType = loginType,
    Credential = credential,
    DeviceId = deviceId,
    ClientVersion = clientVersion,
});
ExpectError("login", loginReply.Error, ProtocolErrorCode.Success);
Expect("login token", loginReply.Token.Length > 0, "token is empty");
Expect("login uid", loginReply.Uid > 0, "uid is invalid");

string token = loginReply.Token;
long uid = loginReply.Uid;
Console.WriteLine($"login ok uid={uid} token={token}");

PrepareRoomConnectionReply prepareReply = await gateClient.PrepareRoomConnectionAsync(new PrepareRoomConnectionRequest
{
    Token = token,
    GameId = GameId,
    Target = TargetRoomWorker,
    RouteId = DefaultWorkerId,
});
ExpectError("prepare room connection", prepareReply.Error, ProtocolErrorCode.Success);
Expect("prepare protocol", prepareReply.DirectProtocol == DirectTransportProtocol.Tcp, $"unexpected protocol={prepareReply.DirectProtocol}");
Expect("prepare endpoint", prepareReply.Host.Length > 0 && prepareReply.Port > 0, "invalid tcp endpoint");
Expect("prepare ticket", prepareReply.ConnectTicket.Length > 0, "connect ticket is empty");
Console.WriteLine($"room direct endpoint: {prepareReply.DirectProtocol} {prepareReply.Host}:{prepareReply.Port}");

PrepareRoomConnectionReply badPrepareReply = await gateClient.PrepareRoomConnectionAsync(new PrepareRoomConnectionRequest
{
    Token = "bad-token",
    GameId = GameId,
    Target = TargetRoomWorker,
    RouteId = DefaultWorkerId,
});
ExpectError("bad prepare token", badPrepareReply.Error, ProtocolErrorCode.Unauthorized);

PrepareRoomConnectionReply unknownRoutePrepareReply = await gateClient.PrepareRoomConnectionAsync(new PrepareRoomConnectionRequest
{
    Token = token,
    GameId = "MissingGame",
    Target = TargetRoomWorker,
    RouteId = DefaultWorkerId,
});
ExpectError("prepare unknown route", unknownRoutePrepareReply.Error, ProtocolErrorCode.RouteNotFound);

await using IRoomClientTransport roomTransport = await RoomClientTransportFactory.ConnectAsync(prepareReply);

await roomTransport.WriteAsync(GamePacketSerializer.Pack(GameMessageIds.Game001RoomConnectRequest, new RoomConnectRequest
{
    ConnectTicket = prepareReply.ConnectTicket,
    RoomId = DefaultRoomId,
}));

GamePacket connectionPacket = await ReadRequiredMessage<GamePacket>(roomTransport, "room connect");
Expect("room connect message", connectionPacket.MessageId == GameMessageIds.Game001RoomConnectionReply, $"unexpected message id={connectionPacket.MessageId}");
RoomConnectionReply connectionReply = GamePacketSerializer.Unpack<RoomConnectionReply>(connectionPacket);
ExpectError("room connect", connectionReply.Error, ProtocolErrorCode.Success);
Expect("room connect uid", connectionReply.Uid == uid, $"expected uid={uid}, actual={connectionReply.Uid}");
Console.WriteLine($"room connected uid={connectionReply.Uid} room={connectionReply.RoomId}");

CreateRoomRsp createReply = await SendRoomCommand<CreateRoomReq, CreateRoomRsp>(
    roomTransport,
    ++requestIndex,
    new CreateRoomReq { RoomId = DefaultRoomId },
    "tcp create room");
ExpectRoomReply("tcp create room", createReply.Error, createReply.Message, ProtocolErrorCode.Success, "created room=room-001");

JoinRoomRsp joinReply = await SendRoomCommand<JoinRoomReq, JoinRoomRsp>(
    roomTransport,
    ++requestIndex,
    new JoinRoomReq { RoomId = DefaultRoomId },
    "tcp join room");
ExpectRoomReply("tcp join room", joinReply.Error, joinReply.Message, ProtocolErrorCode.Success, "joined room=room-001");

RoomPingRsp pingReply = await SendRoomCommand<RoomPingReq, RoomPingRsp>(
    roomTransport,
    ++requestIndex,
    new RoomPingReq
    {
        RoomId = DefaultRoomId,
        ClientTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    },
    "tcp ping room");
ExpectRoomReply("tcp ping room", pingReply.Error, pingReply.Message, ProtocolErrorCode.Success, $"pong uid={uid} room=room-001");

LeaveRoomRsp leaveReply = await SendRoomCommand<LeaveRoomReq, LeaveRoomRsp>(
    roomTransport,
    ++requestIndex,
    new LeaveRoomReq { RoomId = DefaultRoomId },
    "tcp leave room");
ExpectRoomReply("tcp leave room", leaveReply.Error, leaveReply.Message, ProtocolErrorCode.Success, "left room=room-001");

await roomTransport.WriteAsync(new ReqHead
{
    reqHash = UnknownReqHash,
    index = ++requestIndex,
    payload = ArraySegment<byte>.Empty,
});
RspHead unknownReply = await ReadRequiredMessage<RspHead>(roomTransport, "tcp unknown request");
Expect("tcp unknown request", unknownReply.error == NetworkErrorCode.NotSupported, $"unexpected network error={unknownReply.error}");
Console.WriteLine($"tcp unknown request network error ok: {unknownReply.error}");

Console.WriteLine("All client headless checks passed.");

static async Task<TRsp> SendRoomCommand<TReq, TRsp>(
    IRoomClientTransport transport,
    ushort index,
    TReq message,
    string step)
    where TReq : INetworkReq
    where TRsp : INetworkRsp
{
    byte[] payload = MemoryPackSerializer.Serialize(message);
    ReqHead request = new ReqHead
    {
        reqHash = TypeId<TReq>.stableId16,
        index = index,
        payload = new ArraySegment<byte>(payload),
    };

    await transport.WriteAsync(request);

    RspHead reply = await ReadRequiredMessage<RspHead>(transport, step);
    Expect(step, reply.index == request.index, $"unexpected response index={reply.index}");
    Expect(step, reply.reqHash == request.reqHash, $"unexpected req hash={reply.reqHash}");
    Expect(step, reply.rspHash == TypeId<TRsp>.stableId16, $"unexpected rsp hash={reply.rspHash}");
    Expect(step, reply.error == NetworkErrorCode.Success, $"unexpected network error={reply.error} {reply.errorMessage}");
    return MemoryPackSerializer.Deserialize<TRsp>(reply.payload);
}

static async Task<T> ReadRequiredMessage<T>(IRoomClientTransport transport, string step)
    where T : struct
{
    T? message = await transport.ReadAsync<T>();
    if (message == null)
    {
        throw new InvalidOperationException($"{step} failed: connection closed");
    }

    return message.Value;
}

static void ExpectRoomReply(string step, int actualError, string actualMessage, int expectedError, string expectedMessage)
{
    ExpectError(step, actualError, expectedError);
    Expect(step, actualMessage.Contains(expectedMessage, StringComparison.Ordinal), $"message '{actualMessage}' does not contain '{expectedMessage}'");
    Console.WriteLine($"{step} reply: {actualMessage}");
}

static void ExpectError(string step, int actual, int expected)
{
    if (actual != expected)
    {
        throw new InvalidOperationException($"{step} failed: expected error={expected}, actual={actual}");
    }

    Console.WriteLine($"{step} error ok: {actual}");
}

static void Expect(string step, bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException($"{step} failed: {message}");
    }
}

static string ReadOption(string[] args, string argName, string envName, string defaultValue)
{
    for (int i = 0; i < args.Length - 1; i++)
    {
        if (args[i] == argName)
        {
            return args[i + 1];
        }
    }

    string? value = Environment.GetEnvironmentVariable(envName);
    if (!string.IsNullOrWhiteSpace(value))
    {
        return value;
    }

    return defaultValue;
}

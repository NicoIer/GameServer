using Game001.ClientTestApp;
using Game001.Core;
using GameServer.Core.Grpc;
using GameServer.Core.Network;
using GameServer.Core.Protocol;
using Grpc.Net.Client;
using Network;
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

await using ReqRspNetworkClient roomClient = await ReqRspNetworkClient.ConnectAsync(prepareReply);

RoomHandshakeRsp connectionReply = await SendRoomCommand<RoomHandshakeReq, RoomHandshakeRsp>(
    roomClient,
    ++requestIndex,
    new RoomHandshakeReq
    {
        ConnectTicket = prepareReply.ConnectTicket,
    },
    "room handshake");
ExpectError("room handshake", connectionReply.Error, ProtocolErrorCode.Success);
Expect("room handshake uid", connectionReply.Uid == uid, $"expected uid={uid}, actual={connectionReply.Uid}");
Console.WriteLine($"room handshake uid={connectionReply.Uid}");

CreateRoomRsp createReply = await SendRoomCommand<CreateRoomReq, CreateRoomRsp>(
    roomClient,
    ++requestIndex,
    new CreateRoomReq { RoomId = DefaultRoomId },
    "tcp create room");
ExpectRoomReply("tcp create room", createReply.Error, createReply.Message, ProtocolErrorCode.Success, "created room=room-001");

RoomConnectRsp roomConnectReply = await SendRoomCommand<RoomConnectReq, RoomConnectRsp>(
    roomClient,
    ++requestIndex,
    new RoomConnectReq { RoomId = DefaultRoomId },
    "tcp connect room");
ExpectRoomReply("tcp connect room", roomConnectReply.Error, roomConnectReply.Message, ProtocolErrorCode.Success, "connected room=room-001");

JoinRoomRsp joinReply = await SendRoomCommand<JoinRoomReq, JoinRoomRsp>(
    roomClient,
    ++requestIndex,
    new JoinRoomReq(),
    "tcp join room");
ExpectRoomReply("tcp join room", joinReply.Error, joinReply.Message, ProtocolErrorCode.Success, "joined room=room-001");

RoomPingRsp pingReply = await SendRoomCommand<RoomPingReq, RoomPingRsp>(
    roomClient,
    ++requestIndex,
    new RoomPingReq
    {
        ClientTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    },
    "tcp ping room");
ExpectRoomReply("tcp ping room", pingReply.Error, pingReply.Message, ProtocolErrorCode.Success, $"pong uid={uid} room=room-001");

LeaveRoomRsp leaveReply = await SendRoomCommand<LeaveRoomReq, LeaveRoomRsp>(
    roomClient,
    ++requestIndex,
    new LeaveRoomReq(),
    "tcp leave room");
ExpectRoomReply("tcp leave room", leaveReply.Error, leaveReply.Message, ProtocolErrorCode.Success, "left room=room-001");

RspHead unknownReply = await roomClient.SendRawAsync(new ReqHead
{
    reqHash = UnknownReqHash,
    index = ++requestIndex,
    payload = ArraySegment<byte>.Empty,
});
Expect("tcp unknown request", unknownReply.error == NetworkErrorCode.NotSupported, $"unexpected network error={unknownReply.error}");
Console.WriteLine($"tcp unknown request network error ok: {unknownReply.error}");

Console.WriteLine("All client headless checks passed.");

static async Task<TRsp> SendRoomCommand<TReq, TRsp>(
    ReqRspNetworkClient client,
    ushort index,
    TReq message,
    string step)
    where TReq : INetworkReq
    where TRsp : INetworkRsp
{
    TRsp response = await client.SendAsync<TReq, TRsp>(index, message);
    return response;
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

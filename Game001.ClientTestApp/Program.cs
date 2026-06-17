using System.Text;
using GameServer.Core.Grpc;
using GameServer.Core.Protocol;
using Google.Protobuf;
using Grpc.Net.Client;

const string GameId = "Game001";
const string TargetRoom = "room";
const string DefaultRoomId = "room-001";
const int CreateRoomOpcode = 1;
const int JoinRoomOpcode = 2;
const int PingRoomOpcode = 3;
const int UnknownOpcode = 999;

string gateAddress = ReadOption(args, "--gate-address", "GATE_ADDRESS", "http://127.0.0.1:5002");
string loginType = ReadOption(args, "--login-type", "CLIENT_TEST_LOGIN_TYPE", "guest");
string credential = ReadOption(args, "--credential", "CLIENT_TEST_CREDENTIAL", "guest-client-test");
string deviceId = ReadOption(args, "--device-id", "CLIENT_TEST_DEVICE_ID", credential);
string clientVersion = ReadOption(args, "--client-version", "CLIENT_TEST_VERSION", "dev");

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
ExpectError("login", loginReply.Error, ErrorCode.Success);
Expect("login token", loginReply.Token.Length > 0, "token is empty");
Expect("login uid", loginReply.Uid > 0, "uid is invalid");

string token = loginReply.Token;
long uid = loginReply.Uid;
Console.WriteLine($"login ok uid={uid} token={token}");

ForwardReply createReply = await Forward(
    gateClient,
    token,
    gameId: GameId,
    target: TargetRoom,
    routeId: string.Empty,
    opcode: CreateRoomOpcode,
    payload: string.Empty);
ExpectError("create room", createReply.Error, ErrorCode.Success);
ExpectPayloadContains("create room", createReply, "created room=room-001");

ForwardReply joinReply = await Forward(
    gateClient,
    token,
    gameId: GameId,
    target: TargetRoom,
    routeId: DefaultRoomId,
    opcode: JoinRoomOpcode,
    payload: string.Empty);
ExpectError("join room", joinReply.Error, ErrorCode.Success);
ExpectPayloadContains("join room", joinReply, "joined room=room-001");

ForwardReply pingReply = await Forward(
    gateClient,
    token,
    gameId: GameId,
    target: TargetRoom,
    routeId: DefaultRoomId,
    opcode: PingRoomOpcode,
    payload: string.Empty);
ExpectError("ping room", pingReply.Error, ErrorCode.Success);
ExpectPayloadContains("ping room", pingReply, $"pong uid={uid} room=room-001");

ForwardReply badTokenReply = await Forward(
    gateClient,
    token: "bad-token",
    gameId: GameId,
    target: TargetRoom,
    routeId: DefaultRoomId,
    opcode: PingRoomOpcode,
    payload: string.Empty);
ExpectError("bad token", badTokenReply.Error, ErrorCode.Unauthorized);

ForwardReply unknownRouteReply = await Forward(
    gateClient,
    token,
    gameId: "MissingGame",
    target: TargetRoom,
    routeId: DefaultRoomId,
    opcode: PingRoomOpcode,
    payload: string.Empty);
ExpectError("unknown route", unknownRouteReply.Error, ErrorCode.RouteNotFound);

ForwardReply unknownOpcodeReply = await Forward(
    gateClient,
    token,
    gameId: GameId,
    target: TargetRoom,
    routeId: DefaultRoomId,
    opcode: UnknownOpcode,
    payload: string.Empty);
ExpectError("unknown opcode", unknownOpcodeReply.Error, ErrorCode.InvalidRequest);

Console.WriteLine("All client headless checks passed.");

static async Task<ForwardReply> Forward(
    GateService.GateServiceClient gateClient,
    string token,
    string gameId,
    string target,
    string routeId,
    int opcode,
    string payload)
{
    return await gateClient.ForwardAsync(new ForwardRequest
    {
        Token = token,
        Envelope = new ClientEnvelope
        {
            GameId = gameId,
            Target = target,
            RouteId = routeId,
            Opcode = opcode,
            Payload = ByteString.CopyFrom(Encoding.UTF8.GetBytes(payload)),
        },
    });
}

static void ExpectError(string step, int actual, int expected)
{
    if (actual != expected)
    {
        throw new InvalidOperationException($"{step} failed: expected error={expected}, actual={actual}");
    }

    Console.WriteLine($"{step} error ok: {actual}");
}

static void ExpectPayloadContains(string step, ForwardReply reply, string expected)
{
    string payload = Encoding.UTF8.GetString(reply.Payload.ToByteArray());
    Expect(step, payload.Contains(expected, StringComparison.Ordinal), $"payload '{payload}' does not contain '{expected}'");
    Console.WriteLine($"{step} payload: {payload}");
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

using Game001.ClientTestApp;
using GameServer.Core.Grpc;
using GameServer.Core.Network;
using GameServer.Core.Protocol;
using Grpc.Net.Client;

const string GameId = "Game001";
const string TargetRoom = "room";
const string DefaultRoomId = "room-001";
const ushort UnknownMessageId = 999;

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

PrepareRoomConnectionReply prepareReply = await gateClient.PrepareRoomConnectionAsync(new PrepareRoomConnectionRequest
{
    Token = token,
    GameId = GameId,
    Target = TargetRoom,
    RouteId = DefaultRoomId,
});
ExpectError("prepare room connection", prepareReply.Error, ErrorCode.Success);
Expect("prepare protocol", prepareReply.DirectProtocol == DirectTransportProtocol.Tcp, $"unexpected protocol={prepareReply.DirectProtocol}");
Expect("prepare endpoint", prepareReply.Host.Length > 0 && prepareReply.Port > 0, "invalid tcp endpoint");
Expect("prepare ticket", prepareReply.ConnectTicket.Length > 0, "connect ticket is empty");
Console.WriteLine($"room direct endpoint: {prepareReply.DirectProtocol} {prepareReply.Host}:{prepareReply.Port}");

PrepareRoomConnectionReply badPrepareReply = await gateClient.PrepareRoomConnectionAsync(new PrepareRoomConnectionRequest
{
    Token = "bad-token",
    GameId = GameId,
    Target = TargetRoom,
    RouteId = DefaultRoomId,
});
ExpectError("bad prepare token", badPrepareReply.Error, ErrorCode.Unauthorized);

PrepareRoomConnectionReply unknownRoutePrepareReply = await gateClient.PrepareRoomConnectionAsync(new PrepareRoomConnectionRequest
{
    Token = token,
    GameId = "MissingGame",
    Target = TargetRoom,
    RouteId = DefaultRoomId,
});
ExpectError("prepare unknown route", unknownRoutePrepareReply.Error, ErrorCode.RouteNotFound);

await using IRoomClientTransport roomTransport = await RoomClientTransportFactory.ConnectAsync(prepareReply);

await roomTransport.WriteAsync(GamePacketSerializer.Pack(GameMessageIds.Game001RoomConnectRequest, new RoomConnectRequest
{
    ConnectTicket = prepareReply.ConnectTicket,
    RoomId = DefaultRoomId,
}));

GamePacket connectionPacket = await ReadRequiredPacket(roomTransport, "room connect");
Expect("room connect message", connectionPacket.MessageId == GameMessageIds.Game001RoomConnectionReply, $"unexpected message id={connectionPacket.MessageId}");
RoomConnectionReply connectionReply = GamePacketSerializer.Unpack<RoomConnectionReply>(connectionPacket);
ExpectError("room connect", connectionReply.Error, ErrorCode.Success);
Expect("room connect uid", connectionReply.Uid == uid, $"expected uid={uid}, actual={connectionReply.Uid}");
Console.WriteLine($"room connected uid={connectionReply.Uid} room={connectionReply.RoomId}");

RoomCommandReply createReply = await SendRoomCommand(
    roomTransport,
    GameMessageIds.Game001RoomCreateRequest,
    new CreateRoomRequest { RoomId = DefaultRoomId },
    "tcp create room");
ExpectRoomReply("tcp create room", createReply, ErrorCode.Success, "created room=room-001");

RoomCommandReply joinReply = await SendRoomCommand(
    roomTransport,
    GameMessageIds.Game001RoomJoinRequest,
    new JoinRoomRequest { RoomId = DefaultRoomId },
    "tcp join room");
ExpectRoomReply("tcp join room", joinReply, ErrorCode.Success, "joined room=room-001");

RoomCommandReply pingReply = await SendRoomCommand(
    roomTransport,
    GameMessageIds.Game001RoomPingRequest,
    new RoomPingRequest
    {
        RoomId = DefaultRoomId,
        ClientTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    },
    "tcp ping room");
ExpectRoomReply("tcp ping room", pingReply, ErrorCode.Success, $"pong uid={uid} room=room-001");

RoomCommandReply leaveReply = await SendRoomCommand(
    roomTransport,
    GameMessageIds.Game001RoomLeaveRequest,
    new LeaveRoomRequest { RoomId = DefaultRoomId },
    "tcp leave room");
ExpectRoomReply("tcp leave room", leaveReply, ErrorCode.Success, "left room=room-001");

await roomTransport.WriteAsync(new GamePacket(UnknownMessageId, Array.Empty<byte>()));
GamePacket unknownPacket = await ReadRequiredPacket(roomTransport, "tcp unknown message");
Expect("tcp unknown message", unknownPacket.MessageId == GameMessageIds.Game001RoomReply, $"unexpected message id={unknownPacket.MessageId}");
RoomCommandReply unknownReply = GamePacketSerializer.Unpack<RoomCommandReply>(unknownPacket);
ExpectRoomReply("tcp unknown message", unknownReply, ErrorCode.InvalidRequest, "invalid room request");

Console.WriteLine("All client headless checks passed.");

static async Task<RoomCommandReply> SendRoomCommand<T>(
    IRoomClientTransport transport,
    ushort messageId,
    T message,
    string step)
    where T : IGameMessage
{
    GamePacket request = GamePacketSerializer.Pack(messageId, message);
    await transport.WriteAsync(request);

    GamePacket replyPacket = await ReadRequiredPacket(transport, step);
    Expect(step, replyPacket.MessageId == GameMessageIds.Game001RoomReply, $"unexpected message id={replyPacket.MessageId}");
    return GamePacketSerializer.Unpack<RoomCommandReply>(replyPacket);
}

static async Task<GamePacket> ReadRequiredPacket(IRoomClientTransport transport, string step)
{
    GamePacket? packet = await transport.ReadAsync();
    if (packet == null)
    {
        throw new InvalidOperationException($"{step} failed: connection closed");
    }

    return packet.Value;
}

static void ExpectRoomReply(string step, RoomCommandReply reply, int expectedError, string expectedMessage)
{
    ExpectError(step, reply.Error, expectedError);
    Expect(step, reply.Message.Contains(expectedMessage, StringComparison.Ordinal), $"message '{reply.Message}' does not contain '{expectedMessage}'");
    Console.WriteLine($"{step} reply: {reply.Message}");
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

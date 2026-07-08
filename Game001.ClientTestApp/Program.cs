using System.Collections.Concurrent;
using Game001.ClientTestApp;
using Game001.Core;
using GameServer.Core.Grpc;
using GameServer.Core.Network;
using GameServer.Core.Protocol;
using GameServer.Core.Rooms;
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

ListGameWorkersReply workersReply = await gateClient.ListGameWorkersAsync(new ListGameWorkersRequest
{
    Token = token,
    Target = TargetRoomWorker,
});
ExpectError("list game workers", workersReply.Error, ProtocolErrorCode.Success);
Expect(
    "list game workers",
    HasWorker(workersReply, GameId, TargetRoomWorker, DefaultWorkerId),
    $"missing worker {GameId}/{TargetRoomWorker}/{DefaultWorkerId}");
Console.WriteLine($"list game workers ok count={workersReply.Workers.Count}");

ListGameWorkersReply badWorkersReply = await gateClient.ListGameWorkersAsync(new ListGameWorkersRequest
{
    Token = "bad-token",
    Target = TargetRoomWorker,
});
ExpectError("bad list game workers token", badWorkersReply.Error, ProtocolErrorCode.Unauthorized);

PrepareRoomConnectionReply prepareReply = await gateClient.PrepareRoomConnectionAsync(new PrepareRoomConnectionRequest
{
    Token = token,
    GameId = GameId,
    Target = TargetRoomWorker,
    RouteId = DefaultWorkerId,
});
ExpectError("prepare room connection", prepareReply.Error, ProtocolErrorCode.Success);
Expect("prepare protocol", prepareReply.DirectProtocol is DirectTransportProtocol.Tcp or DirectTransportProtocol.Kcp, $"unexpected protocol={prepareReply.DirectProtocol}");
Expect("prepare endpoint", prepareReply.Host.Length > 0 && prepareReply.Port > 0, "invalid direct endpoint");
Expect("prepare ticket", prepareReply.ConnectTicket.Length > 0, "connect ticket is empty");
Console.WriteLine($"room direct endpoint: {prepareReply.DirectProtocol} {prepareReply.Host}:{prepareReply.Port}");
string directStepPrefix = prepareReply.DirectProtocol.ToString().ToLowerInvariant();

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
var roomPushes = new ConcurrentBag<RoomFullStatePush>();
roomClient.RoomPushReceived += push =>
{
    if (push.PushHash == TypeId<RoomFullStatePush>.stableId16)
    {
        roomPushes.Add(MemoryPackSerializer.Deserialize<RoomFullStatePush>(push.Payload));
    }
};

(RoomHandshakeRsp connectionReply, RspHead handshakeHead) = await SendRoomCommand<RoomHandshakeReq, RoomHandshakeRsp>(
    roomClient,
    ++requestIndex,
    new RoomHandshakeReq
    {
        ConnectTicket = prepareReply.ConnectTicket,
    },
    "room handshake");
ExpectNetworkError("room handshake", handshakeHead.error, NetworkErrorCode.Success);
Expect("room handshake message", handshakeHead.errorMessage.Contains("handshake ok", StringComparison.Ordinal), $"unexpected message='{handshakeHead.errorMessage}'");
Expect("room handshake uid", connectionReply.Uid == uid, $"expected uid={uid}, actual={connectionReply.Uid}");
Console.WriteLine($"room handshake uid={connectionReply.Uid}");

(ListRoomsRsp emptyRoomsReply, RspHead listBeforeCreateHead) = await SendRoomCommand<ListRoomsReq, ListRoomsRsp>(
    roomClient,
    ++requestIndex,
    new ListRoomsReq(),
    $"{directStepPrefix} list rooms before create");
ExpectRoomListEmpty($"{directStepPrefix} list rooms before create", emptyRoomsReply, listBeforeCreateHead);

(CreateRoomRsp _, RspHead createHead) = await SendRoomCommand<CreateRoomReq, CreateRoomRsp>(
    roomClient,
    ++requestIndex,
    new CreateRoomReq { RoomId = DefaultRoomId },
    $"{directStepPrefix} create room");
ExpectRoomReply1($"{directStepPrefix} create room", createHead, "created room=room-001");
await ExpectFullStatePush(roomPushes, 1, uid, DefaultRoomId);

(ListRoomsRsp roomsReply, RspHead listAfterCreateHead) = await SendRoomCommand<ListRoomsReq, ListRoomsRsp>(
    roomClient,
    ++requestIndex,
    new ListRoomsReq(),
    $"{directStepPrefix} list rooms after create");
ExpectRoomListContains($"{directStepPrefix} list rooms after create", roomsReply, listAfterCreateHead, DefaultRoomId);

(RoomConnectRsp roomConnectReply, RspHead roomConnectHead) = await SendRoomCommand<RoomConnectReq, RoomConnectRsp>(
    roomClient,
    ++requestIndex,
    new RoomConnectReq { RoomId = DefaultRoomId },
    $"{directStepPrefix} connect room");
ExpectRoomReply($"{directStepPrefix} connect room", roomConnectHead, roomConnectReply.RoomId, DefaultRoomId, "connected room=room-001");

(JoinRoomRsp _, RspHead joinHead) = await SendRoomCommand<JoinRoomReq, JoinRoomRsp>(
    roomClient,
    ++requestIndex,
    new JoinRoomReq(),
    $"{directStepPrefix} join room");
ExpectRoomReply1($"{directStepPrefix} join room", joinHead, "joined room=room-001");
await ExpectFullStatePush(roomPushes, 2, uid, DefaultRoomId);

(RoomPingRsp _, RspHead pingHead) = await SendRoomCommand<RoomPingReq, RoomPingRsp>(
    roomClient,
    ++requestIndex,
    new RoomPingReq
    {
        ClientTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    },
    $"{directStepPrefix} ping room");
ExpectRoomReply1($"{directStepPrefix} ping room", pingHead, $"pong uid={uid} room=room-001");

(LeaveRoomRsp _, RspHead leaveHead) = await SendRoomCommand<LeaveRoomReq, LeaveRoomRsp>(
    roomClient,
    ++requestIndex,
    new LeaveRoomReq(),
    $"{directStepPrefix} leave room");
ExpectRoomReply1($"{directStepPrefix} leave room", leaveHead, "left room=room-001");

RspHead unknownReply = await roomClient.SendRawAsync(new ReqHead
{
    reqHash = UnknownReqHash,
    index = ++requestIndex,
    payload = ArraySegment<byte>.Empty,
});
Expect($"{directStepPrefix} unknown request", unknownReply.error == NetworkErrorCode.NotSupported, $"unexpected network error={unknownReply.error}");
Console.WriteLine($"{directStepPrefix} unknown request network error ok: {unknownReply.error}");

Console.WriteLine("All client headless checks passed.");

static async Task<(TRsp rsp, RspHead head)> SendRoomCommand<TReq, TRsp>(
    ReqRspNetworkClient client,
    ushort index,
    TReq message,
    string step)
    where TReq : INetworkReq
    where TRsp : INetworkRsp
{
    return await client.SendAsync<TReq, TRsp>(index, message);
}

static void ExpectRoomReply1(string step, RspHead head, string expectedMessage)
{
    ExpectNetworkError(step, head.error, NetworkErrorCode.Success);
    Expect(step, head.errorMessage.Contains(expectedMessage, StringComparison.Ordinal), $"message '{head.errorMessage}' does not contain '{expectedMessage}'");
    Console.WriteLine($"{step} reply: {head.errorMessage}");
}

static void ExpectRoomReply(string step, RspHead head, string roomId, string expectedRoomId, string expectedMessage)
{
    ExpectNetworkError(step, head.error, NetworkErrorCode.Success);
    Expect(step, roomId == expectedRoomId, $"expected room={expectedRoomId}, actual={roomId}");
    Expect(step, head.errorMessage.Contains(expectedMessage, StringComparison.Ordinal), $"message '{head.errorMessage}' does not contain '{expectedMessage}'");
    Console.WriteLine($"{step} reply: {head.errorMessage}");
}

static void ExpectRoomListEmpty(string step, ListRoomsRsp rsp, RspHead head)
{
    ExpectNetworkError(step, head.error, NetworkErrorCode.Success);
    Expect(step, rsp.Rooms.Count == 0, $"expected empty room list, actual={rsp.Rooms.Count}");
    Console.WriteLine($"{step} reply: {head.errorMessage}");
}

static void ExpectRoomListContains(string step, ListRoomsRsp rsp, RspHead head, string roomId)
{
    ExpectNetworkError(step, head.error, NetworkErrorCode.Success);
    for (int i = 0; i < rsp.Rooms.Count; i++)
    {
        RoomListItem room = rsp.Rooms.Array![rsp.Rooms.Offset + i];
        if (room.RoomId == roomId)
        {
            Expect(step, room.PlayerCount == 1, $"expected player count=1, actual={room.PlayerCount}");
            Expect(step, room.ConnectionCount == 1, $"expected connection count=1, actual={room.ConnectionCount}");
            Expect(step, room.LifecycleState == (int)RoomLifecycleState.Active, $"expected active room, actual={room.LifecycleState}");
            Console.WriteLine($"{step} room={room.RoomId} players={room.PlayerCount} connections={room.ConnectionCount}");
            return;
        }
    }

    throw new InvalidOperationException($"{step} failed: missing room={roomId}");
}

static bool HasWorker(ListGameWorkersReply reply, string gameId, string target, string routeId)
{
    foreach (GameWorkerInfo worker in reply.Workers)
    {
        if (worker.GameId == gameId &&
            worker.Target == target &&
            worker.RouteId == routeId)
        {
            return true;
        }
    }

    return false;
}

static async Task ExpectFullStatePush(ConcurrentBag<RoomFullStatePush> pushes, int minPushCount, long uid, string roomId)
{
}

static void ExpectError(string step, int actual, int expected)
{
    if (actual != expected)
    {
        throw new InvalidOperationException($"{step} failed: expected error={expected}, actual={actual}");
    }

    Console.WriteLine($"{step} error ok: {actual}");
}

static void ExpectNetworkError(string step, NetworkErrorCode actual, NetworkErrorCode expected)
{
    if (actual != expected)
    {
        throw new InvalidOperationException($"{step} failed: expected network error={expected}, actual={actual}");
    }

    Console.WriteLine($"{step} network error ok: {actual}");
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

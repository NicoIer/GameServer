using GameServer.Core.Network;
using GameServer.Core.Protocol;
using Google.Protobuf;

namespace Game001.Room;

public sealed class Game001RoomPacketHandler
{
    private const string DefaultRoomId = "room-001";

    private readonly Game001RoomState _state;

    public Game001RoomPacketHandler(Game001RoomState state)
    {
        _state = state;
    }

    public GameResponse HandleData(long uid, string routeId, ByteString data)
    {
        GamePacket packet;
        try
        {
            packet = GamePacketSerializer.FromByteString(data);
        }
        catch
        {
            return InvalidRequest();
        }

        return HandlePacket(uid, routeId, packet);
    }

    public GameResponse HandlePacket(long uid, string routeId, GamePacket packet)
    {
        try
        {
            if (packet.MessageId == GameMessageIds.Game001RoomCreateRequest)
            {
                CreateRoomRequest message = GamePacketSerializer.Unpack<CreateRoomRequest>(packet);
                string roomId = ResolveRoomId(message.RoomId, routeId);
                RoomStateResult result = _state.CreateRoom(uid, roomId);
                return Reply(ErrorCode.Success, roomId, result);
            }

            if (packet.MessageId == GameMessageIds.Game001RoomJoinRequest)
            {
                JoinRoomRequest message = GamePacketSerializer.Unpack<JoinRoomRequest>(packet);
                string roomId = ResolveRoomId(message.RoomId, routeId);
                RoomStateResult result = _state.JoinRoom(uid, roomId);
                int error = result.Success ? ErrorCode.Success : ErrorCode.RoomNotFound;
                return Reply(error, roomId, result);
            }

            if (packet.MessageId == GameMessageIds.Game001RoomLeaveRequest)
            {
                LeaveRoomRequest message = GamePacketSerializer.Unpack<LeaveRoomRequest>(packet);
                string roomId = ResolveRoomId(message.RoomId, routeId);
                RoomStateResult result = _state.LeaveRoom(uid, roomId);
                int error = result.Success ? ErrorCode.Success : ErrorCode.RoomNotFound;
                return Reply(error, roomId, result);
            }

            if (packet.MessageId == GameMessageIds.Game001RoomPingRequest)
            {
                RoomPingRequest message = GamePacketSerializer.Unpack<RoomPingRequest>(packet);
                string roomId = ResolveRoomId(message.RoomId, routeId);
                RoomStateResult result = _state.PingRoom(uid, roomId);
                int error = result.Success ? ErrorCode.Success : ErrorCode.RoomNotFound;
                return Reply(error, roomId, result);
            }
        }
        catch
        {
            return InvalidRequest();
        }

        return InvalidRequest();
    }

    private static string ResolveRoomId(string? messageRoomId, string routeId)
    {
        if (!string.IsNullOrWhiteSpace(messageRoomId))
        {
            return messageRoomId;
        }

        if (routeId.Length > 0)
        {
            return routeId;
        }

        return DefaultRoomId;
    }

    private static GameResponse Reply(int error, string roomId, RoomStateResult result)
    {
        RoomCommandReply reply = new RoomCommandReply
        {
            Error = error,
            Success = result.Success,
            Message = result.Message,
            RoomId = roomId,
            PlayerCount = result.PlayerCount,
            ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };

        return new GameResponse
        {
            Error = error,
            Data = GamePacketSerializer.PackToByteString(GameMessageIds.Game001RoomReply, reply),
        };
    }

    private static GameResponse InvalidRequest()
    {
        RoomStateResult result = new RoomStateResult(false, "invalid room request", 0);
        return Reply(ErrorCode.InvalidRequest, string.Empty, result);
    }
}

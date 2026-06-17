using System.Text;
using GameServer.Core.Protocol;
using Google.Protobuf;
using Grpc.Core;

namespace Game001.Room;

public sealed class Game001RoomServiceImpl : GameIngress.GameIngressBase
{
    private const int CreateRoomOpcode = 1;
    private const int JoinRoomOpcode = 2;
    private const int PingRoomOpcode = 3;
    private const string DefaultRoomId = "room-001";

    private readonly Game001RoomState _state;

    public Game001RoomServiceImpl(Game001RoomState state)
    {
        _state = state;
    }

    public override Task<GameResponse> Handle(GameRequest request, ServerCallContext context)
    {
        string roomId = ReadRoomId(request);

        if (request.Opcode == CreateRoomOpcode)
        {
            string result = _state.CreateRoom(request.Uid, DefaultRoomId);
            return Task.FromResult(Success(request.Opcode, result));
        }

        if (request.Opcode == JoinRoomOpcode)
        {
            string result = _state.JoinRoom(request.Uid, roomId);
            if (result.Length == 0)
            {
                return Task.FromResult(new GameResponse { Error = ErrorCode.RoomNotFound, Opcode = request.Opcode });
            }

            return Task.FromResult(Success(request.Opcode, result));
        }

        if (request.Opcode == PingRoomOpcode)
        {
            string result = _state.PingRoom(request.Uid, roomId);
            return Task.FromResult(Success(request.Opcode, result));
        }

        return Task.FromResult(new GameResponse { Error = ErrorCode.InvalidRequest, Opcode = request.Opcode });
    }

    private static string ReadRoomId(GameRequest request)
    {
        if (request.RouteId.Length > 0)
        {
            return request.RouteId;
        }

        string payload = Encoding.UTF8.GetString(request.Payload.ToByteArray());
        if (payload.Length > 0)
        {
            return payload;
        }

        return DefaultRoomId;
    }

    private static GameResponse Success(int opcode, string message)
    {
        return new GameResponse
        {
            Error = ErrorCode.Success,
            Opcode = opcode,
            Payload = ByteString.CopyFromUtf8(message),
        };
    }
}

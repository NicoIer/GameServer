using Game001.Core;
using GameServer.Core.Rooms;
using ProtocolErrorCode = GameServer.Core.Protocol.ErrorCode;
using NetworkErrorCode = Network.ErrorCode;

namespace Game001.Room;

public sealed partial class Game001RoomReqRspHandlers
{
    private readonly RoomConnectionRegistry _connections;
    private readonly RoomRuntimeState _state;
    private readonly RoomFrameAwaiter _frameAwaiter;

    public Game001RoomReqRspHandlers(RoomConnectionRegistry connections, RoomRuntimeState state, RoomFrameAwaiter frameAwaiter)
    {
        _connections = connections;
        _state = state;
        _frameAwaiter = frameAwaiter;
    }

    private bool TryGetContext(int connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg)
    {
        if (_connections.TryGet(connectionId, out context))
        {
            errorCode = NetworkErrorCode.Success;
            errorMsg = string.Empty;
            return true;
        }

        errorCode = NetworkErrorCode.InvalidArgument;
        errorMsg = $"missing room connection context connectionId={connectionId}";
        return false;
    }

    private static void FillResponse(ref CreateRoomRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref JoinRoomRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref LeaveRoomRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static void FillResponse(ref RoomPingRsp rsp, int error, string roomId, RoomStateResult result)
    {
        rsp.Error = error;
        rsp.Success = result.Success;
        rsp.Message = result.Message;
        rsp.RoomId = roomId;
        rsp.PlayerCount = result.PlayerCount;
        rsp.ServerTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public static partial class CreateRoomReqRsp
    {
        public static ValueTask<(CreateRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(
            Game001RoomReqRspHandlers self,
            int connectionId,
            CreateRoomReq req)
        {
            if (!self.TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
            {
                var invalidRsp = new CreateRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
                return new ValueTask<(CreateRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
            }

            RoomStateResult result = self._state.CreateRoom(context.Uid);
            var rsp = new CreateRoomRsp();
            FillResponse(ref rsp, ProtocolErrorCode.Success, self._state.RoomId, result);
            return new ValueTask<(CreateRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, string.Empty));
        }
    }

    public static partial class JoinRoomReqRsp
    {
        public static ValueTask<(JoinRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(
            Game001RoomReqRspHandlers self,
            int connectionId,
            JoinRoomReq req)
        {
            if (!self.TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
            {
                var invalidRsp = new JoinRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
                return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
            }

            RoomStateResult result = self._state.JoinRoom(context.Uid);
            var rsp = new JoinRoomRsp();
            FillResponse(ref rsp, ProtocolErrorCode.Success, self._state.RoomId, result);
            return new ValueTask<(JoinRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, string.Empty));
        }
    }

    public static partial class LeaveRoomReqRsp
    {
        public static ValueTask<(LeaveRoomRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(
            Game001RoomReqRspHandlers self,
            int connectionId,
            LeaveRoomReq req)
        {
            if (!self.TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
            {
                var invalidRsp = new LeaveRoomRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
                return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((invalidRsp, errorCode, errorMsg));
            }

            RoomStateResult result = self._state.LeaveRoom(context.Uid);
            var rsp = new LeaveRoomRsp();
            FillResponse(ref rsp, ProtocolErrorCode.Success, self._state.RoomId, result);
            return new ValueTask<(LeaveRoomRsp, NetworkErrorCode, string)>((rsp, NetworkErrorCode.Success, string.Empty));
        }
    }

    public static partial class RoomPingReqRsp
    {
        public static async ValueTask<(RoomPingRsp rsp, NetworkErrorCode errorCode, string errorMsg)> Handle(
            Game001RoomReqRspHandlers self,
            int connectionId,
            RoomPingReq req)
        {
            await self._frameAwaiter.WaitNextFrameAsync();

            if (!self.TryGetContext(connectionId, out RoomConnectionContext context, out NetworkErrorCode errorCode, out string errorMsg))
            {
                var invalidRsp = new RoomPingRsp { Error = ProtocolErrorCode.InvalidRequest, Message = errorMsg };
                return (invalidRsp, errorCode, errorMsg);
            }

            RoomStateResult result = self._state.PingRoom(context.Uid);
            var rsp = new RoomPingRsp();
            FillResponse(ref rsp, ProtocolErrorCode.Success, self._state.RoomId, result);
            return (rsp, NetworkErrorCode.Success, string.Empty);
        }
    }
}

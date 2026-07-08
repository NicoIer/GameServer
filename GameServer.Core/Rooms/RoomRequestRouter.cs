using MemoryPack;
using Network;
using UnityToolkit;
using NetworkErrorCode = Network.ErrorCode;

namespace GameServer.Core.Rooms;

public enum RoomRequestConnectionAction
{
    None,
    BindRoom,
    ClearRoom,
}

public enum RoomRequestRouteKind
{
    Room,
    Worker,
}

public delegate ValueTask<(TRsp rsp, NetworkErrorCode errorCode, string errorMsg)> RoomWorkerRequestDelegate<TReq, TRsp>(
    int connectionId,
    TReq req,
    RoomConnectionContext context)
    where TReq : INetworkReq
    where TRsp : INetworkRsp;

public delegate ValueTask<RspHead> RoomWorkerRequestInvoker(
    int connectionId,
    ReqHead request,
    RoomConnectionContext context,
    NetworkBuffer responsePayloadWriter);

public readonly record struct RoomRequestRoute(
    RoomRequestRouteKind Kind,
    string RoomId,
    bool CanCreateRoom,
    RoomRequestConnectionAction SuccessConnectionAction,
    NetworkErrorCode RoomNotFoundErrorCode,
    RoomWorkerRequestInvoker? WorkerHandler);

public sealed class RoomRequestRouter
{
    private readonly Dictionary<ushort, RoomRequestRegistration> _roomRoutes = new();
    private readonly Dictionary<ushort, RoomWorkerRequestInvoker> _workerRoutes = new();

    public void Register<TReq>(
        Func<TReq, RoomConnectionContext, string> resolveRoomId,
        bool canCreateRoom,
        RoomRequestConnectionAction successConnectionAction,
        NetworkErrorCode roomNotFoundErrorCode = NetworkErrorCode.InvalidArgument)
        where TReq : INetworkReq
    {
        _roomRoutes[TypeId<TReq>.stableId16] = new RoomRequestRegistration(
            (request, context) =>
            {
                TReq req = MemoryPackSerializer.Deserialize<TReq>(request.payload);
                return resolveRoomId(req, context);
            },
            canCreateRoom,
            successConnectionAction,
            roomNotFoundErrorCode);
    }

    public void RegisterWorker<TReq, TRsp>(RoomWorkerRequestDelegate<TReq, TRsp> handler)
        where TReq : INetworkReq
        where TRsp : INetworkRsp
    {
        ushort reqHash = TypeId<TReq>.stableId16;
        ushort rspHash = TypeId<TRsp>.stableId16;
        _workerRoutes[reqHash] = async (connectionId, request, context, responsePayloadWriter) =>
        {
            TReq req;
            try
            {
                req = MemoryPackSerializer.Deserialize<TReq>(request.payload);
            }
            catch
            {
                return new RspHead(request.index, reqHash, 0, NetworkErrorCode.InvalidArgument, "invalid worker request payload", default);
            }

            (TRsp rsp, NetworkErrorCode errorCode, string errorMsg) result;
            try
            {
                result = await handler(connectionId, req, context);
            }
            catch
            {
                return new RspHead(request.index, reqHash, 0, NetworkErrorCode.InternalError, "worker request failed", default);
            }

            responsePayloadWriter.Reset();
            MemoryPackSerializer.Serialize(responsePayloadWriter, result.rsp);
            return new RspHead(request.index, reqHash, rspHash, result.errorCode, result.errorMsg, responsePayloadWriter.ToArraySegment());
        };
    }

    public bool TryResolve(ReqHead request, RoomConnectionContext context, out RoomRequestRoute route)
    {
        if (_roomRoutes.TryGetValue(request.reqHash, out RoomRequestRegistration registration))
        {
            string roomId = registration.ResolveRoomId(request, context);
            if (string.IsNullOrWhiteSpace(roomId))
            {
                route = default;
                return false;
            }

            route = new RoomRequestRoute(
                RoomRequestRouteKind.Room,
                roomId,
                registration.CanCreateRoom,
                registration.SuccessConnectionAction,
                registration.RoomNotFoundErrorCode,
                null);
            return true;
        }

        if (_workerRoutes.TryGetValue(request.reqHash, out RoomWorkerRequestInvoker? workerHandler))
        {
            route = new RoomRequestRoute(
                RoomRequestRouteKind.Worker,
                string.Empty,
                false,
                RoomRequestConnectionAction.None,
                NetworkErrorCode.InvalidArgument,
                workerHandler);
            return true;
        }

        route = default;
        return false;
    }

    private readonly record struct RoomRequestRegistration(
        Func<ReqHead, RoomConnectionContext, string> ResolveRoomId,
        bool CanCreateRoom,
        RoomRequestConnectionAction SuccessConnectionAction,
        NetworkErrorCode RoomNotFoundErrorCode);
}

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

public readonly record struct RoomRequestRoute(
    string RoomId,
    bool CanCreateRoom,
    RoomRequestConnectionAction SuccessConnectionAction,
    NetworkErrorCode RoomNotFoundErrorCode);

public sealed class RoomRequestRouter
{
    private readonly Dictionary<ushort, RoomRequestRegistration> _routes = new();

    public void Register<TReq>(
        Func<TReq, RoomConnectionContext, string> resolveRoomId,
        bool canCreateRoom,
        RoomRequestConnectionAction successConnectionAction,
        NetworkErrorCode roomNotFoundErrorCode = NetworkErrorCode.InvalidArgument)
        where TReq : INetworkReq
    {
        _routes[TypeId<TReq>.stableId16] = new RoomRequestRegistration(
            (request, context) =>
            {
                TReq req = MemoryPackSerializer.Deserialize<TReq>(request.payload);
                return resolveRoomId(req, context);
            },
            canCreateRoom,
            successConnectionAction,
            roomNotFoundErrorCode);
    }

    public bool TryResolve(ReqHead request, RoomConnectionContext context, out RoomRequestRoute route)
    {
        if (!_routes.TryGetValue(request.reqHash, out RoomRequestRegistration registration))
        {
            route = default;
            return false;
        }

        string roomId = registration.ResolveRoomId(request, context);
        if (string.IsNullOrWhiteSpace(roomId))
        {
            route = default;
            return false;
        }

        route = new RoomRequestRoute(
            roomId,
            registration.CanCreateRoom,
            registration.SuccessConnectionAction,
            registration.RoomNotFoundErrorCode);
        return true;
    }

    private readonly record struct RoomRequestRegistration(
        Func<ReqHead, RoomConnectionContext, string> ResolveRoomId,
        bool CanCreateRoom,
        RoomRequestConnectionAction SuccessConnectionAction,
        NetworkErrorCode RoomNotFoundErrorCode);
}

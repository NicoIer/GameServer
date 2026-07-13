using Friflo.Engine.ECS;
using GameServer.Core.Rooms;

namespace Game001.Room.Runtime;

public sealed class Game001RoomServerRpcAuthority : IRoomServerRpcAuthority
{
    private readonly RoomConnectionRegistry _connections;
    private readonly Game001RoomState _state;

    public Game001RoomServerRpcAuthority(RoomConnectionRegistry connections, Game001RoomState state)
    {
        _connections = connections;
        _state = state;
    }

    public bool TryAuthorize<TComponent>(
        int connectionId,
        int entityId,
        bool requiresAuthority,
        out RoomServerRpcContext context)
        where TComponent : struct, IComponent
    {
        if (!_connections.TryGet(connectionId, out RoomConnectionContext connection) ||
            !string.Equals(connection.RoomId, _state.RoomId, StringComparison.Ordinal) ||
            !_state.ActiveConnectionIds.Contains(connectionId) ||
            _state.PendingFullStateConnections.Contains(connectionId))
        {
            return Reject(connectionId, entityId, "connection_not_ready", out context);
        }

        if (!_state.Entities.TryGetEntityById(entityId, out Entity entity) ||
            entity.IsNull ||
            !entity.HasComponent<TComponent>())
        {
            return Reject(connectionId, entityId, "entity_unavailable", out context);
        }

        if (requiresAuthority &&
            (!Game001RoomRpcEntityResolver.TryGetOwnerUid(entity, out long ownerUid) ||
             ownerUid != connection.Uid))
        {
            return Reject(connectionId, entityId, "authority_denied", out context);
        }

        context = new RoomServerRpcContext(connectionId, connection.Uid, _state.RoomId, entityId);
        return true;
    }

    private static bool Reject(
        int connectionId,
        int entityId,
        string reason,
        out RoomServerRpcContext context)
    {
        global::GameServer.Core.Log.Warning(
            "Room",
            $"event=server_rpc_dropped reason={reason} connectionId={connectionId} entityId={entityId}");
        context = default;
        return false;
    }
}

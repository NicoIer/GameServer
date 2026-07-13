using Friflo.Engine.ECS;

namespace GameServer.Core.Rooms;

public interface IRoomServerRpcAuthority
{
    bool TryAuthorize<TComponent>(
        int connectionId,
        int entityId,
        bool requiresAuthority,
        out RoomServerRpcContext context)
        where TComponent : struct, IComponent;
}

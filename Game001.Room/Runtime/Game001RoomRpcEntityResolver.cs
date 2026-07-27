using Game001.Core.Ecs;
using Friflo.Engine.ECS;

namespace Game001.Room.Runtime;

public static class Game001RoomRpcEntityResolver
{
    public static bool TryGetOwnerUid(Entity entity, out long uid)
    {
        Entity current = entity;
        while (!current.IsNull)
        {
            if (current.HasComponent<UserComponent>())
            {
                uid = current.GetComponent<UserComponent>().Uid;
                return true;
            }

            current = current.Parent;
        }

        uid = 0;
        return false;
    }
}

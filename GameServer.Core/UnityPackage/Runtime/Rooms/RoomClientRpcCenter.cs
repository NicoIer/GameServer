using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;
using GameServer.Core.Ecs;
using MemoryPack;
using UnityToolkit;

namespace GameServer.Core.Rooms
{
    public sealed class RoomClientRpcCenter : IRoomClientRpcRegistry
    {
        private delegate bool RoomClientRpcInvoker(RoomPushHead push);

        private readonly ReplicatedEcsWorld _world;
        private readonly Dictionary<ushort, RoomClientRpcInvoker> _handlers =
            new Dictionary<ushort, RoomClientRpcInvoker>();

        public RoomClientRpcCenter(ReplicatedEcsWorld world)
        {
            _world = world;
        }

        public void Register<TMessage, TComponent>(RoomClientRpcHandler<TMessage> handler)
            where TMessage : IRoomPush, IRoomEntityRpcMessage
            where TComponent : struct, IComponent
        {
            ushort rpcHash = TypeId<TMessage>.stableId16;
            _handlers.Add(rpcHash, push =>
            {
                TMessage message = MemoryPackSerializer.Deserialize<TMessage>(push.Payload);
                if (!_world.HasBaseline ||
                    _world.IsResyncing ||
                    !_world.Store.TryGetEntityById(message.EntityId, out Entity entity) ||
                    entity.IsNull ||
                    !entity.HasComponent<TComponent>())
                {
                    return false;
                }

                handler(message.EntityId, message);
                return true;
            });
        }

        public RoomClientRpcDispatchResult TryHandle(RoomPushHead push)
        {
            if (!_handlers.TryGetValue(push.PushHash, out RoomClientRpcInvoker handler))
            {
                return RoomClientRpcDispatchResult.Unknown;
            }

            return handler(push)
                ? RoomClientRpcDispatchResult.Applied
                : RoomClientRpcDispatchResult.EntityUnavailable;
        }
    }
}

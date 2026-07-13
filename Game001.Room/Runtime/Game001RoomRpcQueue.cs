using Friflo.Engine.ECS;
using GameServer.Core.Rooms;
using MemoryPack;
using UnityToolkit;

namespace Game001.Room.Runtime;

public sealed class Game001RoomRpcQueue : IRoomClientRpcSender
{
    private readonly Game001RoomState _state;
    private readonly RoomConnectionRegistry _connections;
    private readonly RoomPushHub _pushHub;
    private readonly List<PendingRpc> _pending = new();

    public Game001RoomRpcQueue(
        Game001RoomState state,
        RoomConnectionRegistry connections,
        RoomPushHub pushHub)
    {
        _state = state;
        _connections = connections;
        _pushHub = pushHub;
    }

    public int Count => _pending.Count;

    public void SendObservers<TMessage, TComponent>(TMessage message, bool includeOwner)
        where TMessage : IRoomPush, IRoomEntityRpcMessage
        where TComponent : struct, IComponent
    {
        _pending.Add(CreatePending<TMessage, TComponent>(message, 0, includeOwner));
    }

    public void SendTarget<TMessage, TComponent>(int connectionId, TMessage message)
        where TMessage : IRoomPush, IRoomEntityRpcMessage
        where TComponent : struct, IComponent
    {
        _pending.Add(CreatePending<TMessage, TComponent>(message, connectionId, true));
    }

    public void Flush()
    {
        if (_pending.Count == 0)
        {
            return;
        }

        PendingRpc[] calls = _pending.ToArray();
        _pending.Clear();
        foreach (PendingRpc call in calls)
        {
            if (!_state.Entities.TryGetEntityById(call.EntityId, out Entity entity) ||
                entity.IsNull ||
                !call.ValidateEntity(entity))
            {
                continue;
            }

            if (call.TargetConnectionId != 0)
            {
                SendTarget(call, call.TargetConnectionId);
                continue;
            }

            long ownerUid = 0;
            bool hasOwner = Game001RoomRpcEntityResolver.TryGetOwnerUid(entity, out ownerUid);
            foreach (int connectionId in _state.ActiveConnectionIds)
            {
                if (_state.PendingFullStateConnections.Contains(connectionId) ||
                    !_connections.TryGet(connectionId, out RoomConnectionContext context) ||
                    !string.Equals(context.RoomId, _state.RoomId, StringComparison.Ordinal) ||
                    (!call.IncludeOwner && hasOwner && context.Uid == ownerUid))
                {
                    continue;
                }

                _pushHub.Send(connectionId, call.Push);
            }
        }
    }

    private void SendTarget(PendingRpc call, int connectionId)
    {
        if (!_state.ActiveConnectionIds.Contains(connectionId) ||
            _state.PendingFullStateConnections.Contains(connectionId) ||
            !_connections.TryGet(connectionId, out RoomConnectionContext context) ||
            !string.Equals(context.RoomId, _state.RoomId, StringComparison.Ordinal))
        {
            return;
        }

        _pushHub.Send(connectionId, call.Push);
    }

    private static PendingRpc CreatePending<TMessage, TComponent>(
        TMessage message,
        int targetConnectionId,
        bool includeOwner)
        where TMessage : IRoomPush, IRoomEntityRpcMessage
        where TComponent : struct, IComponent
    {
        byte[] payload = MemoryPackSerializer.Serialize(message);
        var push = new RoomPushHead(
            TypeId<TMessage>.stableId16,
            new ArraySegment<byte>(payload));
        return new PendingRpc(
            message.EntityId,
            targetConnectionId,
            includeOwner,
            push,
            static entity => entity.HasComponent<TComponent>());
    }

    private readonly record struct PendingRpc(
        int EntityId,
        int TargetConnectionId,
        bool IncludeOwner,
        RoomPushHead Push,
        Func<Entity, bool> ValidateEntity);
}

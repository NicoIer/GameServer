using MemoryPack;
using Friflo.Engine.ECS;
using UnityToolkit;

namespace GameServer.Core.Rooms;

public delegate void RoomCommandHandler<TCommand>(int connectionId, TCommand command)
    where TCommand : IRoomCommand;

public sealed class RoomCommandServerCenter : IRoomServerRpcRegistry
{
    private delegate void RoomCommandInvoker(int connectionId, RoomCommandHead command);

    private readonly Dictionary<ushort, RoomCommandInvoker> _handlers = new();
    private IRoomServerRpcAuthority? _rpcAuthority;

    public void SetRpcAuthority(IRoomServerRpcAuthority rpcAuthority)
    {
        _rpcAuthority = rpcAuthority;
    }

    public void Register<TCommand>(RoomCommandHandler<TCommand> handler)
        where TCommand : IRoomCommand
    {
        ushort commandHash = TypeId<TCommand>.stableId16;
        _handlers.Add(commandHash, (connectionId, command) =>
        {
            TCommand payload = MemoryPackSerializer.Deserialize<TCommand>(command.Payload);
            handler(connectionId, payload);
        });
    }

    public bool TryHandle(int connectionId, RoomCommandHead command)
    {
        if (!_handlers.TryGetValue(command.CommandHash, out RoomCommandInvoker? handler))
        {
            return false;
        }

        handler(connectionId, command);
        return true;
    }

    public void Register<TMessage, TComponent>(
        bool requiresAuthority,
        RoomServerRpcHandler<TMessage> handler)
        where TMessage : IRoomCommand, IRoomEntityRpcMessage
        where TComponent : struct, IComponent
    {
        IRoomServerRpcAuthority rpcAuthority = _rpcAuthority ??
                                              throw new InvalidOperationException("room ServerRpc authority is not configured");

        ushort commandHash = TypeId<TMessage>.stableId16;
        _handlers.Add(commandHash, (connectionId, command) =>
        {
            TMessage message = MemoryPackSerializer.Deserialize<TMessage>(command.Payload);
            if (!rpcAuthority.TryAuthorize<TComponent>(
                    connectionId,
                    message.EntityId,
                    requiresAuthority,
                    out RoomServerRpcContext context))
            {
                return;
            }

            handler(context, message);
        });
    }
}

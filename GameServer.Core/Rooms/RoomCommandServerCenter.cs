using MemoryPack;
using UnityToolkit;

namespace GameServer.Core.Rooms;

public delegate void RoomCommandHandler<TCommand>(int connectionId, TCommand command)
    where TCommand : IRoomCommand;

public sealed class RoomCommandServerCenter
{
    private delegate void RoomCommandInvoker(int connectionId, RoomCommandHead command);

    private readonly Dictionary<ushort, RoomCommandInvoker> _handlers = new();

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
}

using Game001.Core;
using Game001.Core.Ecs;
using Game001.Core.Generated;
using Game001.Room.Runtime;
using GameServer.Core.Ecs;
using GameServer.Core.Rooms;
using GameServer.Core.Systems;
using Network;

namespace Game001.Room.Systems;

[ExecuteAfter(typeof(RoomLifecycleSystem))]
public sealed class RoomSyncSystem : ISystem
{
    private readonly RoomPushHub _pushHub;
    private readonly Game001RoomState _state;

    public RoomSyncSystem(RoomPushHub pushHub, Game001RoomState state)
    {
        _pushHub = pushHub;
        _state = state;
    }

    public void OnCreate()
    {
    }

    public void Update(in long deltaTimeMs, in int frame, in long timeNowMs)
    {
        FlushDiff();
        FlushFullState();
    }

    public void OnDestroy()
    {
    }

    private void FlushDiff()
    {
        if (!_state.DirtyTracker.HasChanges)
        {
            return;
        }

        NetworkBuffer<EcsEntityChange> entityChangeWriter = NetworkBufferPool<EcsEntityChange>.Shared.Get();
        NetworkBuffer<EcsComponentChange> componentChangeWriter = NetworkBufferPool<EcsComponentChange>.Shared.Get();
        long sourceRevision = _state.WorldRevision;
        long targetRevision = sourceRevision + 1;
        _state.DirtyTracker.Flush(
            sourceRevision,
            targetRevision,
            entityChangeWriter,
            componentChangeWriter,
            out EcsDirtySet dirtySet);

        if (dirtySet.HasChanges)
        {
            _state.WorldRevision = targetRevision;
            var recipients = new List<int>(_state.ActiveConnectionIds.Count);
            foreach (int connectionId in _state.ActiveConnectionIds)
            {
                if (!_state.PendingFullStateConnections.Contains(connectionId))
                {
                    recipients.Add(connectionId);
                }
            }

            if (recipients.Count > 0)
            {
                var diffPush = new RoomDiffStatePush
                {
                    Room = _state.CreateRoomInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                    SourceRevision = dirtySet.SourceRevision,
                    TargetRevision = dirtySet.TargetRevision,
                    EntityChanges = dirtySet.EntityChanges,
                    ComponentChanges = dirtySet.ComponentChanges,
                };
                _pushHub.SendMany(recipients, diffPush);
            }
        }

        NetworkBufferPool<EcsEntityChange>.Shared.Return(entityChangeWriter);
        NetworkBufferPool<EcsComponentChange>.Shared.Return(componentChangeWriter);
    }

    private void FlushFullState()
    {
        if (_state.PendingFullStateConnections.Count == 0)
        {
            return;
        }

        NetworkBuffer<long> playersWriter = NetworkBufferPool<long>.Shared.Get();
        NetworkBuffer<long> disconnectedPlayersWriter = NetworkBufferPool<long>.Shared.Get();
        NetworkBuffer<EcsEntitySnapshot> entityWriter = NetworkBufferPool<EcsEntitySnapshot>.Shared.Get();
        NetworkBuffer<EcsComponentSnapshot> componentWriter = NetworkBufferPool<EcsComponentSnapshot>.Shared.Get();
        NetworkBuffer payloadWriter = NetworkBufferPool.Shared.Get();
        ArraySegment<long> players = WriteSorted(_state.Players, playersWriter);
        ArraySegment<long> disconnectedPlayers = WriteSorted(_state.DisconnectedPlayers, disconnectedPlayersWriter);
        EcsReplicationSerializer.CreateFullState(
            _state.Entities,
            entityWriter,
            componentWriter,
            payloadWriter,
            out ArraySegment<EcsEntitySnapshot> entities);
        var push = new RoomFullStatePush
        {
            Room = _state.CreateRoomInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            WorldRevision = _state.WorldRevision,
            Players = players,
            DisconnectedPlayers = disconnectedPlayers,
            Entities = entities,
        };
        _pushHub.SendMany(_state.PendingFullStateConnections, push);

        _state.PendingFullStateConnections.Clear();
        NetworkBufferPool<long>.Shared.Return(playersWriter);
        NetworkBufferPool<long>.Shared.Return(disconnectedPlayersWriter);
        NetworkBufferPool<EcsEntitySnapshot>.Shared.Return(entityWriter);
        NetworkBufferPool<EcsComponentSnapshot>.Shared.Return(componentWriter);
        NetworkBufferPool.Shared.Return(payloadWriter);
    }

    private static ArraySegment<long> WriteSorted(HashSet<long> values, NetworkBuffer<long> writer)
    {
        writer.Reset();
        foreach (long value in values)
        {
            writer.Write(value);
        }

        ArraySegment<long> segment = writer.ToArraySegment();
        Array.Sort(segment.Array!, segment.Offset, segment.Count);
        return segment;
    }
}

using Game001.Core;
using Game001.Core.Ecs;
using Game001.Core.Generated;
using Game001.Room.Runtime;
using GameServer.Core.Rooms;
using GameServer.Core.Systems;

namespace Game001.Room.Systems;

[ExecuteAfter(typeof(RoomLifecycleSystem))]
public sealed class RoomSyncSystem : ISystem
{
    private readonly RoomPushHub _pushHub;
    private readonly Game001RoomState _state;
    private int _lastDiffFrame;

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
        if (_state.PendingFullStateConnections.Count > 0)
        {
            var push = new RoomFullStatePush
            {
                Room = _state.CreateRoomInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                Players = _state.Players.OrderBy(x => x).ToArray(),
                DisconnectedPlayers = _state.DisconnectedPlayers.OrderBy(x => x).ToArray(),
                Entities = EcsReplicationSerializer.CreateFullState(_state.Entities),
            };
            _pushHub.SendMany(_state.PendingFullStateConnections, push);

            _state.PendingFullStateConnections.Clear();
        }

        if (!_state.DirtyTracker.HasChanges)
        {
            return;
        }

        EcsDirtySet dirtySet = _state.DirtyTracker.Flush(_lastDiffFrame, frame);
        _lastDiffFrame = frame;
        if (!dirtySet.HasChanges || _state.ActiveConnectionIds.Count == 0)
        {
            return;
        }

        var diffPush = new RoomDiffStatePush
        {
            Room = _state.CreateRoomInfo(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
            SourceFrame = dirtySet.SourceFrame,
            TargetFrame = dirtySet.TargetFrame,
            EntityChanges = dirtySet.EntityChanges,
            ComponentChanges = dirtySet.ComponentChanges,
        };
        _pushHub.SendMany(_state.ActiveConnectionIds, diffPush);
    }

    public void OnDestroy()
    {
    }
}

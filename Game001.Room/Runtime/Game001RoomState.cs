using Game001.Core;
using Game001.Core.Ecs;
using Game001.Core.Generated;
using GameServer.Core.Rooms;
using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;

namespace Game001.Room.Runtime;

public sealed class Game001RoomState
{
    private int _lifecycleState = (int)RoomLifecycleState.Created;
    private int _playerCount;
    private readonly Dictionary<long, Entity> _playerEntities = new();

    public string RoomId { get; }
    public EntityStore Entities { get; } = new();
    public SystemRoot EcsSystems { get; }
    public IReadOnlyDictionary<long, Entity> PlayerEntities => _playerEntities;
    public HashSet<long> Players { get; } = new();
    public HashSet<long> DisconnectedPlayers { get; } = new();
    public HashSet<int> ActiveConnectionIds { get; } = new();
    public Dictionary<long, long> DisconnectedPlayerTimesMs { get; } = new();
    public HashSet<int> PendingFullStateConnections { get; } = new();
    public EcsDirtyTracker DirtyTracker { get; }
    public RoomLifecycleState LifecycleState => (RoomLifecycleState)Volatile.Read(ref _lifecycleState);
    public int PlayerCount => Volatile.Read(ref _playerCount);
    public long EmptySinceTimeMs { get; private set; }
    public int Frame { get; private set; }
    public long LastUpdateTimeMs { get; private set; }
    public long WorldRevision { get; set; }

    public Game001RoomState(string roomId)
    {
        RoomId = roomId;
        DirtyTracker = new EcsDirtyTracker(Entities);
        EcsSystems = new SystemRoot(Entities, $"{roomId}.ecs");
    }

    public void SetFrame(long timeNowMs, int frame)
    {
        Frame = frame;
        LastUpdateTimeMs = timeNowMs;
    }

    public void UpdatePlayerCount()
    {
        Volatile.Write(ref _playerCount, Players.Count);
    }

    public Entity GetOrCreatePlayerEntity(long uid)
    {
        if (_playerEntities.TryGetValue(uid, out Entity entity) && !entity.IsNull)
        {
            return entity;
        }

        var player = new RoomPlayerComponent
        {
            Uid = uid,
        };
        entity = Entities.CreateEntity(player);
        _playerEntities[uid] = entity;
        return entity;
    }

    public bool TryGetPlayerEntity(long uid, out Entity entity)
    {
        if (_playerEntities.TryGetValue(uid, out entity) && !entity.IsNull)
        {
            return true;
        }

        entity = default;
        return false;
    }

    public void MarkPlayerConnected(long uid)
    {
        Entity entity = GetOrCreatePlayerEntity(uid);
        if (entity.HasComponent<RoomDisconnectedComponent>())
        {
            entity.RemoveComponent<RoomDisconnectedComponent>();
        }
    }

    public void MarkPlayerDisconnected(long uid, long timeNowMs)
    {
        Entity entity = GetOrCreatePlayerEntity(uid);
        var disconnected = new RoomDisconnectedComponent
        {
            TimeMs = timeNowMs,
        };
        EcsReplicationSerializer.SetReplicatedComponent(entity, disconnected, DirtyTracker);
    }

    public void RemovePlayerEntity(long uid)
    {
        if (!_playerEntities.Remove(uid, out Entity entity) || entity.IsNull)
        {
            return;
        }

        entity.DeleteEntity();
    }

    public void SetActive(long timeNowMs)
    {
        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Active);
        EmptySinceTimeMs = 0;
    }

    public void SetEmpty(long timeNowMs)
    {
        if (LifecycleState == RoomLifecycleState.Empty)
        {
            return;
        }

        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Empty);
        EmptySinceTimeMs = timeNowMs;
    }

    public void SetClosing(long timeNowMs)
    {
        if (LifecycleState == RoomLifecycleState.Closed)
        {
            return;
        }

        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Closing);
    }

    public void SetClosed(long timeNowMs)
    {
        Volatile.Write(ref _lifecycleState, (int)RoomLifecycleState.Closed);
    }

    public RoomInfo CreateRoomInfo(long serverTimeMs)
    {
        return new RoomInfo
        {
            RoomId = RoomId,
            PlayerCount = PlayerCount,
            Frame = Frame,
            ServerTimeMs = serverTimeMs,
        };
    }

    public void Destroy()
    {
        DirtyTracker.Dispose();
    }
}

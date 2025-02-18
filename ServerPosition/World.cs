using System.Buffers;
using GameCore.Position;
using Network;
using Serilog;
using UnityToolkit;
using UnityToolkit.MathTypes;

namespace ServerPosition;

public sealed class World
{
    public long timestamp;
    public List<PositionEntity> entities { get; private set; }
    private PositionEntity[] _entitiesArray;

    public World()
    {
        entities = new List<PositionEntity>();
        _entitiesArray = [];
    }

    public void AddEntity(in int connectId, in uint entityId, in Vector3 spawnPoint)
    {
        entities.Add(new PositionEntity(in connectId, in entityId, in spawnPoint, Quaternion.identity));
        Array.Resize(ref _entitiesArray, entities.Count);
        entities.CopyTo(_entitiesArray);
    }


    public void Remove(int connectId)
    {
        entities.RemoveAll(entity => entity.ownerId == connectId);
        Array.Resize(ref _entitiesArray, entities.Count);
        entities.CopyTo(_entitiesArray);
    }

    public WorldSnapshot GetSnapshot()
    {
        GetSnapshotInternal(out var snapshot);
        return snapshot;
    }

    private void GetSnapshotInternal(out WorldSnapshot snapshot)
    {
        snapshot = new WorldSnapshot
        {
            timestamp = timestamp,
            entities = new ArraySegment<PositionEntity>(_entitiesArray, 0, entities.Count)
        };
    }
}
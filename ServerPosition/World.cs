using System.Buffers;
using GameCore.Position;
using Network;
using Serilog;
using UnityToolkit;
using UnityToolkit.MathTypes;

namespace ServerPosition;



public sealed class World : IPool<WorldSnapshot>
{
    public long timestamp;
    public List<PositionEntity> entities { get; private set; }

    public World()
    {
        entities = new List<PositionEntity>();
    }

    public void AddEntity(in int connectId, in uint entityId, in Vector3 spawnPoint)
    {
        entities.Add(new PositionEntity(in connectId,in entityId,in spawnPoint,Quaternion.identity));
    }


    public void Remove(int connectId)
    {
        entities.RemoveAll(entity => entity.ownerId == connectId);
    }

    public PooledObject<WorldSnapshot> GetSnapshot()
    {
        GetSnapshotInternal(out var snapshot);
        return new PooledObject<WorldSnapshot>(in snapshot, this);
    }

    private void GetSnapshotInternal(out WorldSnapshot snapshot)
    {
        var array = ArrayPool<PositionEntity>.Shared.Rent(entities.Count);
        entities.CopyTo(array);
        snapshot = new WorldSnapshot()
        {
            timestamp = timestamp,
            entities = new ArraySegment<PositionEntity>(array, 0, entities.Count)
        };
    }

    public void Return(in PooledObject<WorldSnapshot> pooledObject)
    {
        Log.Debug("Return WorldSnapshot");
        ArrayPool<PositionEntity>.Shared.Return(pooledObject.message.entities.Array);
    }
}
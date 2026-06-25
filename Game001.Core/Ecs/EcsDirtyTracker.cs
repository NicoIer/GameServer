using Friflo.Engine.ECS;
using Game001.Core.Generated;
using System.Buffers;
using Network;

namespace Game001.Core.Ecs;

public sealed class EcsDirtyTracker : IDisposable
{
    private readonly EntityStore _store;
    private readonly ArrayBufferWriter<byte> _componentPayloadBuffer = new();
    private readonly Dictionary<int, TrackedEntityChange> _entityChanges = new();
    private readonly Dictionary<ComponentChangeKey, TrackedComponentChange> _componentChanges = new();
    private readonly Queue<EntityChangeOrder> _entityChangeOrder = new();
    private readonly Queue<ComponentChangeOrder> _componentChangeOrder = new();
    private long _changeSequence;

    public EcsDirtyTracker(EntityStore store)
    {
        _store = store;
        _store.OnEntityCreate += OnEntityCreate;
        _store.OnEntityDelete += OnEntityDelete;
        _store.OnComponentAdded += OnComponentChanged;
        _store.OnComponentRemoved += OnComponentChanged;
    }

    public bool HasChanges => _entityChanges.Count > 0 || _componentChanges.Count > 0;

    public void MarkComponentUpdated(Entity entity, ushort componentTypeId, byte[] payload)
    {
        int entityId = GetEntityId(entity);
        if (_entityChanges.TryGetValue(entityId, out TrackedEntityChange entityChange) &&
            entityChange.Kind == EcsChangeKind.Delete)
        {
            return;
        }

        var key = new ComponentChangeKey(entityId, componentTypeId);
        if (_componentChanges.TryGetValue(key, out TrackedComponentChange current))
        {
            if (current.Change.Kind == EcsChangeKind.Add)
            {
                current.Change.Payload = new ArraySegment<byte>(payload);
                current.Sequence = NextSequence();
                _componentChanges[key] = current;
                _componentChangeOrder.Enqueue(new ComponentChangeOrder(key, current.Sequence));
                return;
            }
        }

        SetComponentChange(key, new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = EcsChangeKind.Update,
            Payload = new ArraySegment<byte>(payload),
        });
    }

    public void Flush(
        int sourceFrame,
        int targetFrame,
        NetworkBuffer<EcsEntityChange> entityChangeWriter,
        NetworkBuffer<EcsComponentChange> componentChangeWriter,
        out EcsDirtySet set)
    {
        entityChangeWriter.Reset();
        componentChangeWriter.Reset();

        while (_entityChangeOrder.Count > 0)
        {
            EntityChangeOrder item = _entityChangeOrder.Dequeue();
            if (!_entityChanges.TryGetValue(item.EntityId, out TrackedEntityChange current) ||
                current.Sequence != item.Sequence)
            {
                continue;
            }

            entityChangeWriter.Write(new EcsEntityChange
            {
                EntityId = item.EntityId,
                Kind = current.Kind,
            });
        }

        while (_componentChangeOrder.Count > 0)
        {
            ComponentChangeOrder item = _componentChangeOrder.Dequeue();
            if (!_componentChanges.TryGetValue(item.Key, out TrackedComponentChange current) ||
                current.Sequence != item.Sequence)
            {
                continue;
            }

            componentChangeWriter.Write(current.Change);
        }

        _entityChanges.Clear();
        _componentChanges.Clear();

        set = new EcsDirtySet
        {
            SourceFrame = sourceFrame,
            TargetFrame = targetFrame,
            EntityChanges = entityChangeWriter,
            ComponentChanges = componentChangeWriter,
        };
    }

    public void Dispose()
    {
        _store.OnEntityCreate -= OnEntityCreate;
        _store.OnEntityDelete -= OnEntityDelete;
        _store.OnComponentAdded -= OnComponentChanged;
        _store.OnComponentRemoved -= OnComponentChanged;
    }

    private void OnEntityCreate(EntityCreate args)
    {
        int entityId = GetEntityId(args.Entity);
        if (!_entityChanges.ContainsKey(entityId))
        {
            SetEntityChange(entityId, EcsChangeKind.Create);
        }
    }

    private void OnEntityDelete(EntityDelete args)
    {
        int entityId = GetEntityId(args.Entity);
        SetEntityChange(entityId, EcsChangeKind.Delete);

        var removedKeys = new List<ComponentChangeKey>();
        foreach (ComponentChangeKey key in _componentChanges.Keys)
        {
            if (key.EntityId == entityId)
            {
                removedKeys.Add(key);
            }
        }

        foreach (ComponentChangeKey key in removedKeys)
        {
            _componentChanges.Remove(key);
        }
    }

    private void OnComponentChanged(ComponentChanged args)
    {
        if (!EcsReplicationSerializer.TryGetComponentTypeId(args.Type, out ushort componentTypeId))
        {
            return;
        }

        int entityId = (int)args.EntityId;
        if (_entityChanges.TryGetValue(entityId, out TrackedEntityChange entityChange) &&
            entityChange.Kind == EcsChangeKind.Delete)
        {
            return;
        }

        if (args.Action == ComponentChangedAction.Remove)
        {
            MarkComponentRemoved(entityId, componentTypeId);
            return;
        }

        _componentPayloadBuffer.Clear();
        if (!EcsReplicationSerializer.TrySerializeComponent(args.Entity, componentTypeId, _componentPayloadBuffer))
        {
            return;
        }

        var payload = _componentPayloadBuffer.WrittenSpan.ToArray();

        EcsChangeKind kind = args.Action == ComponentChangedAction.Update
            ? EcsChangeKind.Update
            : EcsChangeKind.Add;
        MarkComponentChanged(entityId, componentTypeId, kind, payload);
    }

    private void MarkComponentChanged(int entityId, ushort componentTypeId, EcsChangeKind kind, byte[] payload)
    {
        var key = new ComponentChangeKey(entityId, componentTypeId);
        if (_componentChanges.TryGetValue(key, out TrackedComponentChange current))
        {
            if (current.Change.Kind == EcsChangeKind.Add)
            {
                current.Change.Payload = new ArraySegment<byte>(payload);
                current.Sequence = NextSequence();
                _componentChanges[key] = current;
                _componentChangeOrder.Enqueue(new ComponentChangeOrder(key, current.Sequence));
                return;
            }

            if (current.Change.Kind == EcsChangeKind.Remove && kind == EcsChangeKind.Add)
            {
                kind = EcsChangeKind.Update;
            }
        }

        SetComponentChange(key, new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = kind,
            Payload = new ArraySegment<byte>(payload),
        });
    }

    private void MarkComponentRemoved(int entityId, ushort componentTypeId)
    {
        var key = new ComponentChangeKey(entityId, componentTypeId);
        if (_componentChanges.TryGetValue(key, out TrackedComponentChange current))
        {
            if (current.Change.Kind == EcsChangeKind.Add)
            {
                _componentChanges.Remove(key);
                ClearChangeOrderIfEmpty();
                return;
            }
        }

        SetComponentChange(key, new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = EcsChangeKind.Remove,
            Payload = ArraySegment<byte>.Empty,
        });
    }

    private void SetEntityChange(int entityId, EcsChangeKind kind)
    {
        long sequence = NextSequence();
        _entityChanges[entityId] = new TrackedEntityChange(kind, sequence);
        _entityChangeOrder.Enqueue(new EntityChangeOrder(entityId, sequence));
    }

    private void SetComponentChange(ComponentChangeKey key, EcsComponentChange change)
    {
        long sequence = NextSequence();
        _componentChanges[key] = new TrackedComponentChange(change, sequence);
        _componentChangeOrder.Enqueue(new ComponentChangeOrder(key, sequence));
    }

    private long NextSequence()
    {
        return ++_changeSequence;
    }

    private void ClearChangeOrderIfEmpty()
    {
        if (_entityChanges.Count != 0 || _componentChanges.Count != 0)
        {
            return;
        }

        _entityChangeOrder.Clear();
        _componentChangeOrder.Clear();
    }

    private static int GetEntityId(Entity entity)
    {
        return (int)entity.Pid;
    }

    private readonly struct ComponentChangeKey : IEquatable<ComponentChangeKey>
    {
        public int EntityId { get; }
        public ushort ComponentTypeId { get; }

        public ComponentChangeKey(int entityId, ushort componentTypeId)
        {
            EntityId = entityId;
            ComponentTypeId = componentTypeId;
        }

        public bool Equals(ComponentChangeKey other)
        {
            return EntityId == other.EntityId && ComponentTypeId == other.ComponentTypeId;
        }

        public override bool Equals(object? obj)
        {
            return obj is ComponentChangeKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EntityId, ComponentTypeId);
        }
    }

    private struct TrackedEntityChange
    {
        public EcsChangeKind Kind;
        public long Sequence;

        public TrackedEntityChange(EcsChangeKind kind, long sequence)
        {
            Kind = kind;
            Sequence = sequence;
        }
    }

    private struct TrackedComponentChange
    {
        public EcsComponentChange Change;
        public long Sequence;

        public TrackedComponentChange(EcsComponentChange change, long sequence)
        {
            Change = change;
            Sequence = sequence;
        }
    }

    private readonly struct EntityChangeOrder
    {
        public int EntityId { get; }
        public long Sequence { get; }

        public EntityChangeOrder(int entityId, long sequence)
        {
            EntityId = entityId;
            Sequence = sequence;
        }
    }

    private readonly struct ComponentChangeOrder
    {
        public ComponentChangeKey Key { get; }
        public long Sequence { get; }

        public ComponentChangeOrder(ComponentChangeKey key, long sequence)
        {
            Key = key;
            Sequence = sequence;
        }
    }
}
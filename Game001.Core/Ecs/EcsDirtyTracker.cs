using Friflo.Engine.ECS;
using Game001.Core.Generated;

namespace Game001.Core.Ecs;

public sealed class EcsDirtyTracker : IDisposable
{
    private readonly EntityStore _store;
    private readonly Dictionary<int, EcsChangeKind> _entityChanges = new();
    private readonly Dictionary<ComponentChangeKey, EcsComponentChange> _componentChanges = new();

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
        if (_entityChanges.TryGetValue(entityId, out EcsChangeKind entityChange) &&
            entityChange == EcsChangeKind.Delete)
        {
            return;
        }

        var key = new ComponentChangeKey(entityId, componentTypeId);
        if (_componentChanges.TryGetValue(key, out EcsComponentChange current))
        {
            if (current.Kind == EcsChangeKind.Add)
            {
                current.Payload = payload;
                _componentChanges[key] = current;
                return;
            }
        }

        _componentChanges[key] = new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = EcsChangeKind.Update,
            Payload = payload,
        };
    }

    public EcsDirtySet Flush(int sourceFrame, int targetFrame)
    {
        var entityChanges = new EcsEntityChange[_entityChanges.Count];
        int index = 0;
        foreach (KeyValuePair<int, EcsChangeKind> item in _entityChanges)
        {
            entityChanges[index++] = new EcsEntityChange
            {
                EntityId = item.Key,
                Kind = item.Value,
            };
        }

        var componentChanges = new EcsComponentChange[_componentChanges.Count];
        index = 0;
        foreach (KeyValuePair<ComponentChangeKey, EcsComponentChange> item in _componentChanges)
        {
            componentChanges[index++] = item.Value;
        }

        _entityChanges.Clear();
        _componentChanges.Clear();

        return new EcsDirtySet
        {
            SourceFrame = sourceFrame,
            TargetFrame = targetFrame,
            EntityChanges = entityChanges,
            ComponentChanges = componentChanges,
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
            _entityChanges[entityId] = EcsChangeKind.Create;
        }
    }

    private void OnEntityDelete(EntityDelete args)
    {
        int entityId = GetEntityId(args.Entity);
        _entityChanges[entityId] = EcsChangeKind.Delete;

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
        if (_entityChanges.TryGetValue(entityId, out EcsChangeKind entityChange) &&
            entityChange == EcsChangeKind.Delete)
        {
            return;
        }

        if (args.Action == ComponentChangedAction.Remove)
        {
            MarkComponentRemoved(entityId, componentTypeId);
            return;
        }

        if (!EcsReplicationSerializer.TrySerializeComponent(args.Entity, componentTypeId, out byte[] payload))
        {
            return;
        }

        EcsChangeKind kind = args.Action == ComponentChangedAction.Update
            ? EcsChangeKind.Update
            : EcsChangeKind.Add;
        MarkComponentChanged(entityId, componentTypeId, kind, payload);
    }

    private void MarkComponentChanged(int entityId, ushort componentTypeId, EcsChangeKind kind, byte[] payload)
    {
        var key = new ComponentChangeKey(entityId, componentTypeId);
        if (_componentChanges.TryGetValue(key, out EcsComponentChange current))
        {
            if (current.Kind == EcsChangeKind.Add)
            {
                current.Payload = payload;
                _componentChanges[key] = current;
                return;
            }

            if (current.Kind == EcsChangeKind.Remove && kind == EcsChangeKind.Add)
            {
                kind = EcsChangeKind.Update;
            }
        }

        _componentChanges[key] = new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = kind,
            Payload = payload,
        };
    }

    private void MarkComponentRemoved(int entityId, ushort componentTypeId)
    {
        var key = new ComponentChangeKey(entityId, componentTypeId);
        if (_componentChanges.TryGetValue(key, out EcsComponentChange current))
        {
            if (current.Kind == EcsChangeKind.Add)
            {
                _componentChanges.Remove(key);
                return;
            }
        }

        _componentChanges[key] = new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = EcsChangeKind.Remove,
            Payload = Array.Empty<byte>(),
        };
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
}

public struct EcsDirtySet
{
    public int SourceFrame;
    public int TargetFrame;
    public EcsEntityChange[] EntityChanges;
    public EcsComponentChange[] ComponentChanges;

    public bool HasChanges => EntityChanges.Length > 0 || ComponentChanges.Length > 0;
}

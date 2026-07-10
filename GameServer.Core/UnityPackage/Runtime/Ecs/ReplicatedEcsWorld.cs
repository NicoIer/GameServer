using System;
using System.Collections.Generic;
using Friflo.Engine.ECS;

namespace GameServer.Core.Ecs
{
    public enum EcsWorldApplyResult
    {
        Applied,
        Ignored,
        ResyncRequired,
        ProtocolIncompatible,
    }

    public readonly struct EcsSyncInvalidation
    {
        public readonly EcsWorldApplyResult Result;
        public readonly string Reason;
        public readonly ushort UnknownComponentTypeId;

        public EcsSyncInvalidation(EcsWorldApplyResult result, string reason, ushort unknownComponentTypeId)
        {
            Result = result;
            Reason = reason;
            UnknownComponentTypeId = unknownComponentTypeId;
        }
    }

    public sealed class ReplicatedEcsWorld
    {
        private readonly EcsComponentRegistry _registry;
        private ushort _pendingUnknownComponentTypeId;

        public ReplicatedEcsWorld(EcsComponentRegistry registry)
        {
            _registry = registry;
            Store = new EntityStore();
        }

        public EntityStore Store { get; private set; }
        public long WorldRevision { get; private set; }
        public bool HasBaseline { get; private set; }
        public bool IsResyncing { get; private set; }
        public bool IsProtocolCompatible { get; private set; } = true;
        public string LastError { get; private set; } = string.Empty;

        public event Action<EntityStore> WorldReplaced;
        public event Action<int> EntityChanged;
        public event Action<EcsSyncInvalidation> SyncInvalidated;

        public EcsWorldApplyResult ApplyFullState(long worldRevision, ArraySegment<EcsEntitySnapshot> entities)
        {
            if (!IsProtocolCompatible)
            {
                return EcsWorldApplyResult.ProtocolIncompatible;
            }

            bool wasResyncing = IsResyncing;
            EntityStore replacement;
            try
            {
                replacement = CreateFullState(entities);
            }
            catch (UnknownComponentException exception)
            {
                return InvalidateUnknownComponent(exception.ComponentTypeId, wasResyncing);
            }
            catch (Exception exception)
            {
                return Invalidate($"invalid ECS full state: {exception.Message}", 0, wasResyncing);
            }

            Store = replacement;
            WorldRevision = worldRevision;
            HasBaseline = true;
            IsResyncing = false;
            _pendingUnknownComponentTypeId = 0;
            LastError = string.Empty;
            WorldReplaced?.Invoke(Store);
            return EcsWorldApplyResult.Applied;
        }

        public EcsWorldApplyResult ApplyDiff(
            long sourceRevision,
            long targetRevision,
            ArraySegment<EcsEntityChange> entityChanges,
            ArraySegment<EcsComponentChange> componentChanges)
        {
            if (!IsProtocolCompatible)
            {
                return EcsWorldApplyResult.ProtocolIncompatible;
            }

            if (targetRevision <= WorldRevision)
            {
                return EcsWorldApplyResult.Ignored;
            }

            if (!HasBaseline)
            {
                return Invalidate("received ECS diff before full-state baseline");
            }

            if (IsResyncing)
            {
                return EcsWorldApplyResult.ResyncRequired;
            }

            if (sourceRevision != WorldRevision)
            {
                return Invalidate(
                    $"ECS diff revision gap current={WorldRevision} source={sourceRevision} target={targetRevision}");
            }

            DecodedComponentChange[] decodedComponents;
            try
            {
                decodedComponents = DecodeComponentChanges(componentChanges);
                ValidateDiff(entityChanges, decodedComponents);
            }
            catch (UnknownComponentException exception)
            {
                return InvalidateUnknownComponent(exception.ComponentTypeId, false);
            }
            catch (Exception exception)
            {
                return Invalidate($"invalid ECS diff: {exception.Message}");
            }

            try
            {
                ApplyEntityCreates(entityChanges);
                ApplyComponentChanges(decodedComponents);
                ApplyReparents(entityChanges);
                ApplyEntityDeletes(entityChanges);
            }
            catch (Exception exception)
            {
                return DiscardAndInvalidate($"failed to apply ECS diff: {exception.Message}");
            }

            WorldRevision = targetRevision;
            NotifyChangedEntities(entityChanges, componentChanges);
            return EcsWorldApplyResult.Applied;
        }

        public EcsWorldApplyResult MarkInvalid(string reason)
        {
            if (!IsProtocolCompatible)
            {
                return EcsWorldApplyResult.ProtocolIncompatible;
            }

            return Invalidate(reason);
        }

        public void Reset()
        {
            Store = new EntityStore();
            WorldRevision = 0;
            HasBaseline = false;
            IsResyncing = false;
            IsProtocolCompatible = true;
            _pendingUnknownComponentTypeId = 0;
            LastError = string.Empty;
            WorldReplaced?.Invoke(Store);
        }

        private EntityStore CreateFullState(ArraySegment<EcsEntitySnapshot> snapshots)
        {
            var replacement = new EntityStore();
            for (int i = 0; i < snapshots.Count; i++)
            {
                EcsEntitySnapshot snapshot = snapshots.Array[snapshots.Offset + i];
                if (snapshot.EntityId <= 0)
                {
                    throw new InvalidOperationException($"invalid entity id={snapshot.EntityId}");
                }

                replacement.CreateEntity(snapshot.EntityId);
            }

            for (int i = 0; i < snapshots.Count; i++)
            {
                EcsEntitySnapshot snapshot = snapshots.Array[snapshots.Offset + i];
                Entity entity = GetEntity(replacement, snapshot.EntityId);
                for (int componentIndex = 0; componentIndex < snapshot.Components.Count; componentIndex++)
                {
                    EcsComponentSnapshot component =
                        snapshot.Components.Array[snapshot.Components.Offset + componentIndex];
                    ComponentRegistration registration = GetRegistration(component.ComponentTypeId);
                    object value = registration.Deserialize(component.Payload);
                    registration.Set(entity, value);
                }
            }

            for (int i = 0; i < snapshots.Count; i++)
            {
                EcsEntitySnapshot snapshot = snapshots.Array[snapshots.Offset + i];
                if (snapshot.ParentEntityId == 0)
                {
                    continue;
                }

                Entity parent = GetEntity(replacement, snapshot.ParentEntityId);
                Entity child = GetEntity(replacement, snapshot.EntityId);
                parent.AddChild(child);
            }

            return replacement;
        }

        private DecodedComponentChange[] DecodeComponentChanges(ArraySegment<EcsComponentChange> changes)
        {
            var result = new DecodedComponentChange[changes.Count];
            for (int i = 0; i < changes.Count; i++)
            {
                EcsComponentChange change = changes.Array[changes.Offset + i];
                ComponentRegistration registration = GetRegistration(change.ComponentTypeId);
                object value = null;
                if (change.Kind == EcsChangeKind.Add || change.Kind == EcsChangeKind.Update)
                {
                    value = registration.Deserialize(change.Payload);
                }
                else if (change.Kind != EcsChangeKind.Remove)
                {
                    throw new InvalidOperationException(
                        $"invalid component change kind={change.Kind} entity={change.EntityId}");
                }

                result[i] = new DecodedComponentChange(change, registration, value);
            }

            return result;
        }

        private void ValidateDiff(
            ArraySegment<EcsEntityChange> entityChanges,
            DecodedComponentChange[] componentChanges)
        {
            var created = new HashSet<int>();
            var deleted = new HashSet<int>();
            for (int i = 0; i < entityChanges.Count; i++)
            {
                EcsEntityChange change = entityChanges.Array[entityChanges.Offset + i];
                if (change.EntityId <= 0)
                {
                    throw new InvalidOperationException($"invalid entity id={change.EntityId}");
                }

                if (change.Kind == EcsChangeKind.Create)
                {
                    if (TryGetEntity(Store, change.EntityId, out _) || !created.Add(change.EntityId))
                    {
                        throw new InvalidOperationException($"duplicate entity create id={change.EntityId}");
                    }
                }
                else if (change.Kind == EcsChangeKind.Delete)
                {
                    if (!TryGetEntity(Store, change.EntityId, out _) || !deleted.Add(change.EntityId))
                    {
                        throw new InvalidOperationException($"missing entity delete id={change.EntityId}");
                    }
                }
                else if (change.Kind != EcsChangeKind.Reparent)
                {
                    throw new InvalidOperationException(
                        $"invalid entity change kind={change.Kind} entity={change.EntityId}");
                }
            }

            for (int i = 0; i < entityChanges.Count; i++)
            {
                EcsEntityChange change = entityChanges.Array[entityChanges.Offset + i];
                if (change.Kind != EcsChangeKind.Create && change.Kind != EcsChangeKind.Reparent)
                {
                    continue;
                }

                ValidateLiveEntity(change.EntityId, created, deleted);
                if (change.ParentEntityId != 0)
                {
                    if (change.ParentEntityId == change.EntityId)
                    {
                        throw new InvalidOperationException($"entity cannot parent itself id={change.EntityId}");
                    }

                    ValidateLiveEntity(change.ParentEntityId, created, deleted);
                }
            }

            var componentKeys = new HashSet<ComponentChangeKey>();
            for (int i = 0; i < componentChanges.Length; i++)
            {
                DecodedComponentChange decoded = componentChanges[i];
                EcsComponentChange change = decoded.Change;
                ValidateLiveEntity(change.EntityId, created, deleted);

                var key = new ComponentChangeKey(change.EntityId, change.ComponentTypeId);
                if (!componentKeys.Add(key))
                {
                    throw new InvalidOperationException(
                        $"duplicate component change entity={change.EntityId} type={change.ComponentTypeId}");
                }

                bool hasComponent = false;
                if (!created.Contains(change.EntityId))
                {
                    hasComponent = decoded.Registration.Has(GetEntity(Store, change.EntityId));
                }

                if (change.Kind == EcsChangeKind.Add && hasComponent)
                {
                    throw new InvalidOperationException(
                        $"duplicate component add entity={change.EntityId} type={change.ComponentTypeId}");
                }

                if ((change.Kind == EcsChangeKind.Update || change.Kind == EcsChangeKind.Remove) && !hasComponent)
                {
                    throw new InvalidOperationException(
                        $"missing component {change.Kind} entity={change.EntityId} type={change.ComponentTypeId}");
                }
            }

            ValidateParentGraph(entityChanges, created, deleted);
        }

        private void ValidateParentGraph(
            ArraySegment<EcsEntityChange> entityChanges,
            HashSet<int> created,
            HashSet<int> deleted)
        {
            var parents = new Dictionary<int, int>();
            foreach (Entity entity in Store.Entities)
            {
                if (deleted.Contains(entity.Id))
                {
                    continue;
                }

                Entity parent = entity.Parent;
                parents[entity.Id] = parent.IsNull ? 0 : parent.Id;
            }

            foreach (int entityId in created)
            {
                parents.Add(entityId, 0);
            }

            for (int i = 0; i < entityChanges.Count; i++)
            {
                EcsEntityChange change = entityChanges.Array[entityChanges.Offset + i];
                if (change.Kind == EcsChangeKind.Create || change.Kind == EcsChangeKind.Reparent)
                {
                    parents[change.EntityId] = change.ParentEntityId;
                }
            }

            var path = new HashSet<int>();
            foreach (int entityId in parents.Keys)
            {
                path.Clear();
                int current = entityId;
                while (current != 0)
                {
                    if (!path.Add(current))
                    {
                        throw new InvalidOperationException($"entity parent cycle id={entityId}");
                    }

                    int currentEntityId = current;
                    if (!parents.TryGetValue(currentEntityId, out current))
                    {
                        throw new InvalidOperationException($"missing parent entity id={currentEntityId}");
                    }
                }
            }
        }

        private void ValidateLiveEntity(int entityId, HashSet<int> created, HashSet<int> deleted)
        {
            if (deleted.Contains(entityId) ||
                (!created.Contains(entityId) && !TryGetEntity(Store, entityId, out _)))
            {
                throw new InvalidOperationException($"missing entity id={entityId}");
            }
        }

        private void ApplyEntityCreates(ArraySegment<EcsEntityChange> changes)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                EcsEntityChange change = changes.Array[changes.Offset + i];
                if (change.Kind == EcsChangeKind.Create)
                {
                    Store.CreateEntity(change.EntityId);
                }
            }
        }

        private void ApplyComponentChanges(DecodedComponentChange[] changes)
        {
            for (int i = 0; i < changes.Length; i++)
            {
                DecodedComponentChange decoded = changes[i];
                EcsComponentChange change = decoded.Change;
                Entity entity = GetEntity(Store, change.EntityId);
                bool hasComponent = decoded.Registration.Has(entity);

                if (change.Kind == EcsChangeKind.Add)
                {
                    if (hasComponent)
                    {
                        throw new InvalidOperationException(
                            $"duplicate component add entity={change.EntityId} type={change.ComponentTypeId}");
                    }

                    decoded.Registration.Set(entity, decoded.Value);
                }
                else if (change.Kind == EcsChangeKind.Update)
                {
                    if (!hasComponent)
                    {
                        throw new InvalidOperationException(
                            $"missing component update entity={change.EntityId} type={change.ComponentTypeId}");
                    }

                    decoded.Registration.Set(entity, decoded.Value);
                }
                else
                {
                    if (!hasComponent)
                    {
                        throw new InvalidOperationException(
                            $"missing component remove entity={change.EntityId} type={change.ComponentTypeId}");
                    }

                    decoded.Registration.Remove(entity);
                }
            }
        }

        private void ApplyReparents(ArraySegment<EcsEntityChange> changes)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                EcsEntityChange change = changes.Array[changes.Offset + i];
                if (change.Kind != EcsChangeKind.Create && change.Kind != EcsChangeKind.Reparent)
                {
                    continue;
                }

                Entity child = GetEntity(Store, change.EntityId);
                if (change.ParentEntityId != 0)
                {
                    Entity parent = GetEntity(Store, change.ParentEntityId);
                    parent.AddChild(child);
                    continue;
                }

                Entity oldParent = child.Parent;
                if (!oldParent.IsNull)
                {
                    oldParent.RemoveChild(child);
                }
            }
        }

        private void ApplyEntityDeletes(ArraySegment<EcsEntityChange> changes)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                EcsEntityChange change = changes.Array[changes.Offset + i];
                if (change.Kind == EcsChangeKind.Delete)
                {
                    GetEntity(Store, change.EntityId).DeleteEntity();
                }
            }
        }

        private void NotifyChangedEntities(
            ArraySegment<EcsEntityChange> entityChanges,
            ArraySegment<EcsComponentChange> componentChanges)
        {
            if (EntityChanged == null)
            {
                return;
            }

            var changedIds = new HashSet<int>();
            for (int i = 0; i < entityChanges.Count; i++)
            {
                changedIds.Add(entityChanges.Array[entityChanges.Offset + i].EntityId);
            }

            for (int i = 0; i < componentChanges.Count; i++)
            {
                changedIds.Add(componentChanges.Array[componentChanges.Offset + i].EntityId);
            }

            foreach (int entityId in changedIds)
            {
                EntityChanged(entityId);
            }
        }

        private ComponentRegistration GetRegistration(ushort componentTypeId)
        {
            if (!_registry.TryGet(componentTypeId, out ComponentRegistration registration))
            {
                throw new UnknownComponentException(componentTypeId);
            }

            return registration;
        }

        private EcsWorldApplyResult InvalidateUnknownComponent(ushort componentTypeId, bool resyncFullState)
        {
            if (resyncFullState ||
                (IsResyncing && _pendingUnknownComponentTypeId == componentTypeId))
            {
                IsResyncing = false;
                IsProtocolCompatible = false;
                LastError = $"unknown replicated ECS component type id={componentTypeId}";
                SyncInvalidated?.Invoke(new EcsSyncInvalidation(
                    EcsWorldApplyResult.ProtocolIncompatible,
                    LastError,
                    componentTypeId));
                return EcsWorldApplyResult.ProtocolIncompatible;
            }

            _pendingUnknownComponentTypeId = componentTypeId;
            return Invalidate($"unknown replicated ECS component type id={componentTypeId}", componentTypeId);
        }

        private EcsWorldApplyResult Invalidate(
            string reason,
            ushort unknownComponentTypeId = 0,
            bool forceNotification = false)
        {
            bool notify = forceNotification || !IsResyncing;
            IsResyncing = true;
            LastError = reason;
            if (notify)
            {
                SyncInvalidated?.Invoke(new EcsSyncInvalidation(
                    EcsWorldApplyResult.ResyncRequired,
                    reason,
                    unknownComponentTypeId));
            }

            return EcsWorldApplyResult.ResyncRequired;
        }

        private EcsWorldApplyResult DiscardAndInvalidate(string reason)
        {
            Store = new EntityStore();
            WorldRevision = 0;
            HasBaseline = false;
            WorldReplaced?.Invoke(Store);
            return Invalidate(reason);
        }

        private static Entity GetEntity(EntityStore store, int entityId)
        {
            if (!TryGetEntity(store, entityId, out Entity entity))
            {
                throw new InvalidOperationException($"missing entity id={entityId}");
            }

            return entity;
        }

        private static bool TryGetEntity(EntityStore store, int entityId, out Entity entity)
        {
            return store.TryGetEntityById(entityId, out entity) && !entity.IsNull;
        }

        private readonly struct DecodedComponentChange
        {
            public readonly EcsComponentChange Change;
            public readonly ComponentRegistration Registration;
            public readonly object Value;

            public DecodedComponentChange(
                EcsComponentChange change,
                ComponentRegistration registration,
                object value)
            {
                Change = change;
                Registration = registration;
                Value = value;
            }
        }

        private readonly struct ComponentChangeKey : IEquatable<ComponentChangeKey>
        {
            private readonly int _entityId;
            private readonly ushort _componentTypeId;

            public ComponentChangeKey(int entityId, ushort componentTypeId)
            {
                _entityId = entityId;
                _componentTypeId = componentTypeId;
            }

            public bool Equals(ComponentChangeKey other)
            {
                return _entityId == other._entityId && _componentTypeId == other._componentTypeId;
            }

            public override bool Equals(object obj)
            {
                return obj is ComponentChangeKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_entityId * 397) ^ _componentTypeId;
                }
            }
        }

        private sealed class UnknownComponentException : Exception
        {
            public readonly ushort ComponentTypeId;

            public UnknownComponentException(ushort componentTypeId)
            {
                ComponentTypeId = componentTypeId;
            }
        }
    }
}

using Friflo.Engine.ECS;
using Game001.Core.Ecs;
using Game001.Core.Generated;
using GameServer.Core.Ecs;
using MemoryPack;
using UnityToolkit;

namespace GameServer.Core.Tests;

[MemoryPackable]
public partial struct TestPositionComponent : IComponent
{
    public float X;
    public float Y;
}

[MemoryPackable]
public partial struct TestHealthComponent : IComponent
{
    public int Value;
}

public sealed class ReplicatedEcsWorldTests
{
    private EcsComponentRegistry _registry = null!;
    private ReplicatedEcsWorld _world = null!;
    private ushort _positionTypeId;
    private ushort _healthTypeId;

    [SetUp]
    public void SetUp()
    {
        _registry = new EcsComponentRegistry();
        _positionTypeId = _registry.Register<TestPositionComponent>();
        _healthTypeId = _registry.Register<TestHealthComponent>();
        _world = new ReplicatedEcsWorld(_registry);
    }

    [Test]
    public void ApplyFullState_RebuildsExactIdsComponentsAndParentAtomically()
    {
        int worldReplacedCount = 0;
        _world.WorldReplaced += _ => worldReplacedCount++;
        ArraySegment<EcsEntitySnapshot> baseline = Segment(
            Snapshot(
                204,
                101,
                Component(_healthTypeId, new TestHealthComponent { Value = 75 })),
            Snapshot(
                101,
                0,
                Component(_positionTypeId, new TestPositionComponent { X = 12.5f, Y = -3.25f })));

        EcsWorldApplyResult result = _world.ApplyFullState(7, baseline);

        Entity parent = _world.Store.GetEntityById(101);
        Entity child = _world.Store.GetEntityById(204);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));
            Assert.That(_world.WorldRevision, Is.EqualTo(7));
            Assert.That(_world.HasBaseline, Is.True);
            Assert.That(worldReplacedCount, Is.EqualTo(1));
            Assert.That(_world.Store.Entities.Count, Is.EqualTo(2));
            Assert.That(parent.Id, Is.EqualTo(101));
            Assert.That(parent.GetComponent<TestPositionComponent>().X, Is.EqualTo(12.5f));
            Assert.That(child.Id, Is.EqualTo(204));
            Assert.That(child.GetComponent<TestHealthComponent>().Value, Is.EqualTo(75));
            Assert.That(child.Parent.Id, Is.EqualTo(101));
        });

        EntityStore acceptedStore = _world.Store;
        ArraySegment<EcsEntitySnapshot> invalidReplacement = Segment(
            Snapshot(300, 999, Component(
                _positionTypeId,
                new TestPositionComponent { X = 1, Y = 2 })));

        result = _world.ApplyFullState(8, invalidReplacement);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(_world.Store, Is.SameAs(acceptedStore));
            Assert.That(_world.WorldRevision, Is.EqualTo(7));
            Assert.That(_world.Store.GetEntityById(101).IsNull, Is.False);
            Assert.That(_world.Store.GetEntityById(300).IsNull, Is.True);
            Assert.That(_world.IsResyncing, Is.True);
            Assert.That(worldReplacedCount, Is.EqualTo(1));
        });
    }

    [Test]
    public void ApplyDiff_AppliesCreateComponentsReparentAndDeleteInProtocolOrder()
    {
        EcsWorldApplyResult result = _world.ApplyFullState(10, Segment(
            Snapshot(
                1,
                0,
                Component(_positionTypeId, new TestPositionComponent { X = 1, Y = 1 }),
                Component(_healthTypeId, new TestHealthComponent { Value = 100 })),
            Snapshot(2),
            Snapshot(
                3,
                1,
                Component(_positionTypeId, new TestPositionComponent { X = 3, Y = 3 })),
            Snapshot(4)));
        Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));

        var changedEntities = new HashSet<int>();
        _world.EntityChanged += changedEntities.Add;

        ArraySegment<EcsEntityChange> entityChanges = Segment(
            EntityChange(5, EcsChangeKind.Create, 2),
            EntityChange(3, EcsChangeKind.Reparent, 2),
            EntityChange(4, EcsChangeKind.Delete));
        ArraySegment<EcsComponentChange> componentChanges = Segment(
            ComponentChange(
                5,
                _positionTypeId,
                EcsChangeKind.Add,
                new TestPositionComponent { X = 5, Y = -5 }),
            ComponentChange(
                3,
                _positionTypeId,
                EcsChangeKind.Update,
                new TestPositionComponent { X = 30, Y = 40 }),
            ComponentRemove(1, _healthTypeId));

        result = _world.ApplyDiff(10, 11, entityChanges, componentChanges);

        Entity entity1 = _world.Store.GetEntityById(1);
        Entity entity3 = _world.Store.GetEntityById(3);
        Entity entity5 = _world.Store.GetEntityById(5);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));
            Assert.That(_world.WorldRevision, Is.EqualTo(11));
            Assert.That(_world.Store.Entities.Count, Is.EqualTo(4));
            Assert.That(entity1.HasComponent<TestHealthComponent>(), Is.False);
            Assert.That(entity3.GetComponent<TestPositionComponent>().X, Is.EqualTo(30));
            Assert.That(entity3.GetComponent<TestPositionComponent>().Y, Is.EqualTo(40));
            Assert.That(entity3.Parent.Id, Is.EqualTo(2));
            Assert.That(entity5.GetComponent<TestPositionComponent>().X, Is.EqualTo(5));
            Assert.That(entity5.Parent.Id, Is.EqualTo(2));
            Assert.That(_world.Store.GetEntityById(4).IsNull, Is.True);
            Assert.That(changedEntities, Is.EquivalentTo(new[] { 1, 3, 4, 5 }));
        });

        result = _world.ApplyDiff(
            11,
            12,
            Segment(EntityChange(3, EcsChangeKind.Reparent)),
            Segment<EcsComponentChange>());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));
            Assert.That(_world.WorldRevision, Is.EqualTo(12));
            Assert.That(_world.Store.GetEntityById(3).Parent.IsNull, Is.True);
        });
    }

    [Test]
    public void ApplyDiff_IgnoresDuplicateAndCoalescesRevisionGapResync()
    {
        EcsWorldApplyResult result = _world.ApplyFullState(20, Segment(
            Snapshot(
                1,
                0,
                Component(_positionTypeId, new TestPositionComponent { X = 1, Y = 2 }))));
        Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));

        var invalidations = new List<EcsSyncInvalidation>();
        _world.SyncInvalidated += invalidations.Add;

        result = _world.ApplyDiff(
            19,
            20,
            Segment(EntityChange(99, EcsChangeKind.Create)),
            Segment(ComponentChange(
                99,
                ushort.MaxValue,
                EcsChangeKind.Add,
                new TestPositionComponent { X = 99, Y = 99 })));

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Ignored));
            Assert.That(_world.WorldRevision, Is.EqualTo(20));
            Assert.That(_world.IsResyncing, Is.False);
            Assert.That(invalidations, Is.Empty);
        });

        result = _world.ApplyDiff(
            21,
            22,
            Segment(EntityChange(2, EcsChangeKind.Create)),
            Segment<EcsComponentChange>());
        EcsWorldApplyResult repeatedGap = _world.ApplyDiff(
            21,
            22,
            Segment(EntityChange(2, EcsChangeKind.Create)),
            Segment<EcsComponentChange>());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(repeatedGap, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(_world.Store.GetEntityById(2).IsNull, Is.True);
            Assert.That(_world.WorldRevision, Is.EqualTo(20));
            Assert.That(_world.IsResyncing, Is.True);
            Assert.That(invalidations, Has.Count.EqualTo(1));
        });

        result = _world.ApplyFullState(25, Segment(Snapshot(9)));
        EcsWorldApplyResult nextDiff = _world.ApplyDiff(
            25,
            26,
            Segment(EntityChange(10, EcsChangeKind.Create, 9)),
            Segment<EcsComponentChange>());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));
            Assert.That(nextDiff, Is.EqualTo(EcsWorldApplyResult.Applied));
            Assert.That(_world.WorldRevision, Is.EqualTo(26));
            Assert.That(_world.IsResyncing, Is.False);
            Assert.That(_world.Store.GetEntityById(10).Parent.Id, Is.EqualTo(9));
        });
    }

    [Test]
    public void UnknownComponentInResyncFullStateMarksProtocolIncompatible()
    {
        var invalidations = new List<EcsSyncInvalidation>();
        _world.SyncInvalidated += invalidations.Add;
        const ushort unknownTypeId = 65000;
        ArraySegment<EcsEntitySnapshot> unknownState = Segment(
            Snapshot(
                1,
                0,
                new EcsComponentSnapshot
                {
                    ComponentTypeId = unknownTypeId,
                    Payload = Segment<byte>(),
                }));

        EcsWorldApplyResult firstResult = _world.ApplyFullState(1, unknownState);
        EcsWorldApplyResult secondResult = _world.ApplyFullState(1, unknownState);

        Assert.Multiple(() =>
        {
            Assert.That(firstResult, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(secondResult, Is.EqualTo(EcsWorldApplyResult.ProtocolIncompatible));
            Assert.That(_world.HasBaseline, Is.False);
            Assert.That(_world.IsProtocolCompatible, Is.False);
            Assert.That(_world.IsResyncing, Is.False);
            Assert.That(invalidations, Has.Count.EqualTo(2));
            Assert.That(invalidations[0].UnknownComponentTypeId, Is.EqualTo(unknownTypeId));
            Assert.That(invalidations[1].Result, Is.EqualTo(EcsWorldApplyResult.ProtocolIncompatible));
        });
    }

    [Test]
    public void UnknownComponentDiffDoesNotMutateWorldAndRequestsOneResync()
    {
        EcsWorldApplyResult result = _world.ApplyFullState(30, Segment(
            Snapshot(
                1,
                0,
                Component(_positionTypeId, new TestPositionComponent { X = 1, Y = 2 }))));
        Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));

        var invalidations = new List<EcsSyncInvalidation>();
        _world.SyncInvalidated += invalidations.Add;
        const ushort unknownTypeId = 65000;
        var unknownChange = new EcsComponentChange
        {
            EntityId = 1,
            ComponentTypeId = unknownTypeId,
            Kind = EcsChangeKind.Update,
            Payload = Segment<byte>(),
        };

        result = _world.ApplyDiff(
            30,
            31,
            Segment<EcsEntityChange>(),
            Segment(unknownChange));
        EcsWorldApplyResult repeatedResult = _world.ApplyDiff(
            30,
            31,
            Segment<EcsEntityChange>(),
            Segment(unknownChange));

        TestPositionComponent position = _world.Store.GetEntityById(1).GetComponent<TestPositionComponent>();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(repeatedResult, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(_world.WorldRevision, Is.EqualTo(30));
            Assert.That(position.X, Is.EqualTo(1));
            Assert.That(position.Y, Is.EqualTo(2));
            Assert.That(_world.IsResyncing, Is.True);
            Assert.That(invalidations, Has.Count.EqualTo(1));
            Assert.That(invalidations[0].UnknownComponentTypeId, Is.EqualTo(unknownTypeId));
        });
    }

    [Test]
    public void DamagedComponentPayloadDoesNotAdvanceOrMutateWorldAndRequestsOneResync()
    {
        EcsWorldApplyResult result = _world.ApplyFullState(3, Segment(
            Snapshot(
                7,
                0,
                Component(_positionTypeId, new TestPositionComponent { X = 7, Y = 8 }))));
        Assert.That(result, Is.EqualTo(EcsWorldApplyResult.Applied));

        var invalidations = new List<EcsSyncInvalidation>();
        _world.SyncInvalidated += invalidations.Add;
        var damagedChange = new EcsComponentChange
        {
            EntityId = 7,
            ComponentTypeId = _positionTypeId,
            Kind = EcsChangeKind.Update,
            Payload = Segment<byte>(),
        };

        result = _world.ApplyDiff(
            3,
            4,
            Segment<EcsEntityChange>(),
            Segment(damagedChange));
        EcsWorldApplyResult repeatedResult = _world.ApplyDiff(
            3,
            4,
            Segment<EcsEntityChange>(),
            Segment(damagedChange));

        TestPositionComponent position = _world.Store.GetEntityById(7).GetComponent<TestPositionComponent>();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(repeatedResult, Is.EqualTo(EcsWorldApplyResult.ResyncRequired));
            Assert.That(_world.WorldRevision, Is.EqualTo(3));
            Assert.That(position.X, Is.EqualTo(7));
            Assert.That(position.Y, Is.EqualTo(8));
            Assert.That(invalidations, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void RegistryTypeIdAndWireMessagesMatchStableMemoryPackContract()
    {
        var registry = new EcsComponentRegistry();
        ushort registeredTypeId = registry.Register<TestPositionComponent>();
        ushort repeatedTypeId = registry.Register<TestPositionComponent>();

        Assert.Multiple(() =>
        {
            Assert.That(registeredTypeId, Is.EqualTo(TypeId<TestPositionComponent>.stableId16));
            Assert.That(registeredTypeId, Is.EqualTo(EcsComponentTypeId.Get<TestPositionComponent>()));
            Assert.That(repeatedTypeId, Is.EqualTo(registeredTypeId));
            Assert.That(registry.Count, Is.EqualTo(1));
            Assert.That(registry.Contains(registeredTypeId), Is.True);
        });

        ushort roomPlayerTypeId = registry.Register<RoomPlayerComponent>();
        ushort roomDisconnectedTypeId = registry.Register<RoomDisconnectedComponent>();
        Assert.Multiple(() =>
        {
            Assert.That(roomPlayerTypeId, Is.EqualTo(EcsReplicationSerializer.RoomPlayerComponentTypeId));
            Assert.That(roomDisconnectedTypeId,
                Is.EqualTo(EcsReplicationSerializer.RoomDisconnectedComponentTypeId));
        });

        var snapshot = Snapshot(
            88,
            77,
            Component(
                registeredTypeId,
                new TestPositionComponent { X = 8.5f, Y = -9.25f }));
        var change = ComponentChange(
            88,
            registeredTypeId,
            EcsChangeKind.Update,
            new TestPositionComponent { X = 10, Y = 11 });

        byte[] snapshotBytes = MemoryPackSerializer.Serialize(snapshot);
        byte[] changeBytes = MemoryPackSerializer.Serialize(change);
        EcsEntitySnapshot roundTripSnapshot = MemoryPackSerializer.Deserialize<EcsEntitySnapshot>(snapshotBytes);
        EcsComponentChange roundTripChange = MemoryPackSerializer.Deserialize<EcsComponentChange>(changeBytes);
        TestPositionComponent snapshotPosition = MemoryPackSerializer.Deserialize<TestPositionComponent>(
            roundTripSnapshot.Components.Array![roundTripSnapshot.Components.Offset].Payload);
        TestPositionComponent changedPosition = MemoryPackSerializer.Deserialize<TestPositionComponent>(
            roundTripChange.Payload);

        Assert.Multiple(() =>
        {
            Assert.That(roundTripSnapshot.EntityId, Is.EqualTo(88));
            Assert.That(roundTripSnapshot.ParentEntityId, Is.EqualTo(77));
            Assert.That(roundTripSnapshot.Components.Count, Is.EqualTo(1));
            Assert.That(roundTripSnapshot.Components.Array![roundTripSnapshot.Components.Offset].ComponentTypeId,
                Is.EqualTo(registeredTypeId));
            Assert.That(snapshotPosition.X, Is.EqualTo(8.5f));
            Assert.That(snapshotPosition.Y, Is.EqualTo(-9.25f));
            Assert.That(roundTripChange.EntityId, Is.EqualTo(88));
            Assert.That(roundTripChange.Kind, Is.EqualTo(EcsChangeKind.Update));
            Assert.That(changedPosition.X, Is.EqualTo(10));
            Assert.That(changedPosition.Y, Is.EqualTo(11));
        });
    }

    private static EcsEntitySnapshot Snapshot(
        int entityId,
        int parentEntityId = 0,
        params EcsComponentSnapshot[] components)
    {
        return new EcsEntitySnapshot
        {
            EntityId = entityId,
            ParentEntityId = parentEntityId,
            Components = Segment(components),
        };
    }

    private static EcsComponentSnapshot Component<T>(ushort componentTypeId, T value)
    {
        return new EcsComponentSnapshot
        {
            ComponentTypeId = componentTypeId,
            Payload = Pack(value),
        };
    }

    private static EcsEntityChange EntityChange(
        int entityId,
        EcsChangeKind kind,
        int parentEntityId = 0)
    {
        return new EcsEntityChange
        {
            EntityId = entityId,
            ParentEntityId = parentEntityId,
            Kind = kind,
        };
    }

    private static EcsComponentChange ComponentChange<T>(
        int entityId,
        ushort componentTypeId,
        EcsChangeKind kind,
        T value)
    {
        return new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = kind,
            Payload = Pack(value),
        };
    }

    private static EcsComponentChange ComponentRemove(int entityId, ushort componentTypeId)
    {
        return new EcsComponentChange
        {
            EntityId = entityId,
            ComponentTypeId = componentTypeId,
            Kind = EcsChangeKind.Remove,
            Payload = Segment<byte>(),
        };
    }

    private static ArraySegment<byte> Pack<T>(T value)
    {
        return Segment(MemoryPackSerializer.Serialize(value));
    }

    private static ArraySegment<T> Segment<T>(params T[] values)
    {
        return new ArraySegment<T>(values);
    }
}

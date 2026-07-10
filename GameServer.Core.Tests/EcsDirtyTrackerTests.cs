using Friflo.Engine.ECS;
using Game001.Core;
using Game001.Core.Ecs;
using GameServer.Core.Ecs;
using Network;

namespace GameServer.Core.Tests;

public sealed class EcsDirtyTrackerTests
{
    [Test]
    public void CreateAndAddChildAreMergedIntoParentedCreate()
    {
        var store = new EntityStore();
        Entity parent = store.CreateEntity(10);
        using var tracker = new EcsDirtyTracker(store);

        Entity child = store.CreateEntity(20);
        parent.AddChild(child);

        EcsDirtySet set = Flush(tracker, 4, 5);

        Assert.Multiple(() =>
        {
            Assert.That(set.SourceRevision, Is.EqualTo(4));
            Assert.That(set.TargetRevision, Is.EqualTo(5));
            Assert.That(set.EntityChanges.Count, Is.EqualTo(1));
            Assert.That(set.EntityChanges.Array![set.EntityChanges.Offset].EntityId, Is.EqualTo(20));
            Assert.That(set.EntityChanges.Array![set.EntityChanges.Offset].Kind,
                Is.EqualTo(EcsChangeKind.Create));
            Assert.That(set.EntityChanges.Array![set.EntityChanges.Offset].ParentEntityId, Is.EqualTo(10));
            Assert.That(set.ComponentChanges.Count, Is.Zero);
            Assert.That(tracker.HasChanges, Is.False);
        });
    }

    [Test]
    public void ReparentAndRemoveFromHierarchyProduceOnlyLatestParent()
    {
        var store = new EntityStore();
        Entity firstParent = store.CreateEntity(1);
        Entity secondParent = store.CreateEntity(2);
        Entity child = store.CreateEntity(3);
        firstParent.AddChild(child);
        using var tracker = new EcsDirtyTracker(store);

        secondParent.AddChild(child);

        EcsDirtySet reparent = Flush(tracker, 10, 11);
        Assert.Multiple(() =>
        {
            Assert.That(reparent.EntityChanges.Count, Is.EqualTo(1));
            Assert.That(reparent.EntityChanges.Array![reparent.EntityChanges.Offset].EntityId, Is.EqualTo(3));
            Assert.That(reparent.EntityChanges.Array![reparent.EntityChanges.Offset].Kind,
                Is.EqualTo(EcsChangeKind.Reparent));
            Assert.That(reparent.EntityChanges.Array![reparent.EntityChanges.Offset].ParentEntityId,
                Is.EqualTo(2));
            Assert.That(tracker.HasChanges, Is.False);
        });

        secondParent.RemoveChild(child);

        EcsDirtySet detached = Flush(tracker, 11, 12);
        Assert.Multiple(() =>
        {
            Assert.That(detached.EntityChanges.Count, Is.EqualTo(1));
            Assert.That(detached.EntityChanges.Array![detached.EntityChanges.Offset].Kind,
                Is.EqualTo(EcsChangeKind.Reparent));
            Assert.That(detached.EntityChanges.Array![detached.EntityChanges.Offset].ParentEntityId,
                Is.Zero);
            Assert.That(tracker.HasChanges, Is.False);
        });
    }

    [Test]
    public void CreateThenDeleteCancelsAndExistingDeleteDropsComponentChanges()
    {
        var store = new EntityStore();
        Entity existing = store.CreateEntity(1);
        using var tracker = new EcsDirtyTracker(store);

        Entity transient = store.CreateEntity(2);
        transient.AddComponent(new RoomPlayerComponent { Uid = 2 });
        transient.DeleteEntity();

        Assert.That(tracker.HasChanges, Is.False);
        EcsDirtySet cancelled = Flush(tracker, 1, 2);
        Assert.Multiple(() =>
        {
            Assert.That(cancelled.HasChanges, Is.False);
            Assert.That(cancelled.EntityChanges.Count, Is.Zero);
            Assert.That(cancelled.ComponentChanges.Count, Is.Zero);
        });

        existing.AddComponent(new RoomPlayerComponent { Uid = 1 });
        existing.DeleteEntity();

        EcsDirtySet deleted = Flush(tracker, 2, 3);
        Assert.Multiple(() =>
        {
            Assert.That(deleted.EntityChanges.Count, Is.EqualTo(1));
            Assert.That(deleted.EntityChanges.Array![deleted.EntityChanges.Offset].EntityId,
                Is.EqualTo(1));
            Assert.That(deleted.EntityChanges.Array![deleted.EntityChanges.Offset].Kind,
                Is.EqualTo(EcsChangeKind.Delete));
            Assert.That(deleted.ComponentChanges.Count, Is.Zero);
            Assert.That(tracker.HasChanges, Is.False);
        });
    }

    private static EcsDirtySet Flush(EcsDirtyTracker tracker, long sourceRevision, long targetRevision)
    {
        var entityWriter = new NetworkBuffer<EcsEntityChange>();
        var componentWriter = new NetworkBuffer<EcsComponentChange>();
        tracker.Flush(sourceRevision, targetRevision, entityWriter, componentWriter, out EcsDirtySet set);
        return set;
    }
}

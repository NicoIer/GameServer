using Game001.Core;
using Game001.Room.Runtime;
using Game001.Room.Systems;
using GameServer.Core.Ecs;
using GameServer.Core.Rooms;
using MemoryPack;
using UnityToolkit;

namespace GameServer.Core.Tests;

public sealed class RoomSyncSystemTests
{
    [Test]
    public void RoomTickSendsOnlyChangesAndKeepsFullAndDiffRevisionsContinuous()
    {
        var state = new Game001RoomState("sync-test");
        var pushHub = new RoomPushHub();
        var syncSystem = new RoomSyncSystem(pushHub, state);
        RoomFullStatePush fullState = default;
        RoomDiffStatePush diffState = default;
        int fullCount = 0;
        int diffCount = 0;

        pushHub.Register(1, push =>
        {
            if (push.PushHash == TypeId<RoomFullStatePush>.stableId16)
            {
                fullState = MemoryPackSerializer.Deserialize<RoomFullStatePush>(push.Payload);
                fullCount++;
                return;
            }

            if (push.PushHash == TypeId<RoomDiffStatePush>.stableId16)
            {
                diffState = MemoryPackSerializer.Deserialize<RoomDiffStatePush>(push.Payload);
                diffCount++;
            }
        });

        try
        {
            syncSystem.Update(0, 0, 0);
            Assert.That(pushHub.SentCount, Is.Zero);

            state.ActiveConnectionIds.Add(1);
            state.PendingFullStateConnections.Add(1);
            state.Entities.CreateEntity(50);
            state.SetFrame(20, 1);
            syncSystem.Update(20, 1, 20);

            Assert.Multiple(() =>
            {
                Assert.That(fullCount, Is.EqualTo(1));
                Assert.That(diffCount, Is.Zero);
                Assert.That(fullState.WorldRevision, Is.EqualTo(1));
                Assert.That(fullState.Entities.Count, Is.EqualTo(1));
                Assert.That(fullState.Entities.Array![fullState.Entities.Offset].EntityId, Is.EqualTo(50));
                Assert.That(fullState.Entities.Array![fullState.Entities.Offset].Components.Count, Is.Zero);
                Assert.That(state.WorldRevision, Is.EqualTo(1));
            });

            state.Entities.CreateEntity(60);
            state.SetFrame(40, 2);
            syncSystem.Update(20, 2, 40);

            Assert.Multiple(() =>
            {
                Assert.That(fullCount, Is.EqualTo(1));
                Assert.That(diffCount, Is.EqualTo(1));
                Assert.That(diffState.SourceRevision, Is.EqualTo(fullState.WorldRevision));
                Assert.That(diffState.TargetRevision, Is.EqualTo(2));
                Assert.That(diffState.EntityChanges.Count, Is.EqualTo(1));
                Assert.That(diffState.EntityChanges.Array![diffState.EntityChanges.Offset].EntityId,
                    Is.EqualTo(60));
                Assert.That(diffState.EntityChanges.Array![diffState.EntityChanges.Offset].Kind,
                    Is.EqualTo(EcsChangeKind.Create));
            });

            long sentCount = pushHub.SentCount;
            state.SetFrame(60, 3);
            syncSystem.Update(20, 3, 60);
            Assert.That(pushHub.SentCount, Is.EqualTo(sentCount));
        }
        finally
        {
            pushHub.Unregister(1);
            state.Destroy();
        }
    }
}

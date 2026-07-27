using Friflo.Engine.ECS;
using Game001.Core.Ecs;
using Game001.Room.Runtime;
using Game001.Room.Systems;
using GameServer.Core.Ecs;
using GameServer.Core.Rooms;
using MemoryPack;
using UnityToolkit;

namespace GameServer.Core.Tests;

[MemoryPackable]
public partial struct TestServerRpcMessage : IRoomCommand, IRoomEntityRpcMessage
{
    public int EntityId;
    public int Value;

    int IRoomEntityRpcMessage.EntityId => EntityId;
}

[MemoryPackable]
public partial struct TestClientRpcMessage : IRoomPush, IRoomEntityRpcMessage
{
    public int EntityId;
    public int Value;

    int IRoomEntityRpcMessage.EntityId => EntityId;
}

[TestFixture]
public sealed class RoomRpcTests
{
    [Test]
    public void ServerRpcChecksComponentParentAuthorityAndBypass()
    {
        var connections = new RoomConnectionRegistry();
        var pushHub = new RoomPushHub();
        var state = new Game001RoomState("rpc-room", connections, pushHub);
        int ownerConnectionId = connections.Add(101, state.RoomId);
        int otherConnectionId = connections.Add(202, state.RoomId);
        state.ActiveConnectionIds.Add(ownerConnectionId);
        state.ActiveConnectionIds.Add(otherConnectionId);
        Entity owner = state.Entities.CreateEntity(new UserComponent { Uid = 101 });
        Entity child = state.Entities.CreateEntity(new TestPositionComponent { X = 1 });
        owner.AddChild(child);
        Entity unowned = state.Entities.CreateEntity(new TestPositionComponent { X = 2 });
        var authority = new Game001RoomServerRpcAuthority(connections, state);
        var center = new RoomCommandServerCenter();
        center.SetRpcAuthority(authority);
        int invokeCount = 0;
        RoomServerRpcContext handledContext = default;
        center.Register<TestServerRpcMessage, TestPositionComponent>(true, (context, _) =>
        {
            invokeCount++;
            handledContext = context;
        });

        Send(center, ownerConnectionId, child.Id, 1);
        Send(center, otherConnectionId, child.Id, 2);
        Send(center, ownerConnectionId, owner.Id, 3);
        Send(center, ownerConnectionId, unowned.Id, 4);

        Assert.Multiple(() =>
        {
            Assert.That(invokeCount, Is.EqualTo(1));
            Assert.That(handledContext.ConnectionId, Is.EqualTo(ownerConnectionId));
            Assert.That(handledContext.Uid, Is.EqualTo(101));
            Assert.That(handledContext.EntityId, Is.EqualTo(child.Id));
        });

        var bypassCenter = new RoomCommandServerCenter();
        bypassCenter.SetRpcAuthority(authority);
        bool bypassInvoked = false;
        bypassCenter.Register<TestServerRpcMessage, TestPositionComponent>(false, (_, _) => bypassInvoked = true);
        Send(bypassCenter, otherConnectionId, unowned.Id, 5);
        Assert.That(bypassInvoked, Is.True);
        state.Destroy();
    }

    [Test]
    public void ClientRpcQueueFiltersOwnerTargetsConnectionAndKeepsFifo()
    {
        var connections = new RoomConnectionRegistry();
        var pushHub = new RoomPushHub();
        var state = new Game001RoomState("rpc-room", connections, pushHub);
        int ownerConnectionId = connections.Add(101, state.RoomId);
        int observerConnectionId = connections.Add(202, state.RoomId);
        int pendingConnectionId = connections.Add(303, state.RoomId);
        int otherRoomConnectionId = connections.Add(404, "other-room");
        state.ActiveConnectionIds.Add(ownerConnectionId);
        state.ActiveConnectionIds.Add(observerConnectionId);
        state.ActiveConnectionIds.Add(pendingConnectionId);
        state.ActiveConnectionIds.Add(otherRoomConnectionId);
        state.PendingFullStateConnections.Add(pendingConnectionId);
        Entity owner = state.Entities.CreateEntity(new UserComponent { Uid = 101 });
        Entity child = state.Entities.CreateEntity(new TestPositionComponent { X = 1 });
        owner.AddChild(child);
        var ownerValues = new List<int>();
        var observerValues = new List<int>();
        var rejectedValues = new List<int>();
        pushHub.Register(ownerConnectionId, push => ownerValues.Add(ReadClientValue(push)));
        pushHub.Register(observerConnectionId, push => observerValues.Add(ReadClientValue(push)));
        pushHub.Register(pendingConnectionId, push => rejectedValues.Add(ReadClientValue(push)));
        pushHub.Register(otherRoomConnectionId, push => rejectedValues.Add(ReadClientValue(push)));

        state.RpcQueue.SendObservers<TestClientRpcMessage, TestPositionComponent>(
            new TestClientRpcMessage { EntityId = child.Id, Value = 1 },
            false);
        state.RpcQueue.SendObservers<TestClientRpcMessage, TestPositionComponent>(
            new TestClientRpcMessage { EntityId = child.Id, Value = 2 },
            true);
        state.RpcQueue.SendTarget<TestClientRpcMessage, TestPositionComponent>(
            ownerConnectionId,
            new TestClientRpcMessage { EntityId = child.Id, Value = 3 });
        state.RpcQueue.SendTarget<TestClientRpcMessage, TestPositionComponent>(
            pendingConnectionId,
            new TestClientRpcMessage { EntityId = child.Id, Value = 4 });
        state.RpcQueue.SendTarget<TestClientRpcMessage, TestPositionComponent>(
            otherRoomConnectionId,
            new TestClientRpcMessage { EntityId = child.Id, Value = 5 });

        Assert.That(state.RpcQueue.Count, Is.EqualTo(5));
        state.RpcQueue.Flush();

        Assert.Multiple(() =>
        {
            Assert.That(ownerValues, Is.EqualTo(new[] { 2, 3 }));
            Assert.That(observerValues, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(rejectedValues, Is.Empty);
            Assert.That(state.RpcQueue.Count, Is.Zero);
        });
        state.Destroy();
    }

    [Test]
    public void ClientDispatcherRequiresBaselineEntityAndComponent()
    {
        var registry = new EcsComponentRegistry();
        ushort componentTypeId = registry.Register<TestPositionComponent>();
        var world = new ReplicatedEcsWorld(registry);
        byte[] componentPayload = MemoryPackSerializer.Serialize(new TestPositionComponent { X = 5 });
        var components = new[]
        {
            new EcsComponentSnapshot
            {
                ComponentTypeId = componentTypeId,
                Payload = new ArraySegment<byte>(componentPayload),
            },
        };
        var entities = new[]
        {
            new EcsEntitySnapshot
            {
                EntityId = 44,
                Components = new ArraySegment<EcsComponentSnapshot>(components),
            },
        };
        Assert.That(
            world.ApplyFullState(1, new ArraySegment<EcsEntitySnapshot>(entities)),
            Is.EqualTo(EcsWorldApplyResult.Applied));
        var center = new RoomClientRpcCenter(world);
        int receivedValue = 0;
        center.Register<TestClientRpcMessage, TestPositionComponent>((_, message) =>
        {
            if (message.Value == 99)
            {
                throw new InvalidOperationException("test handler failure");
            }

            receivedValue = message.Value;
        });
        RoomPushHead push = CreateClientPush(44, 9);

        RoomClientRpcDispatchResult result = center.TryHandle(push);
        RoomClientRpcDispatchResult unavailable = center.TryHandle(CreateClientPush(999, 10));
        Assert.That(
            () => center.TryHandle(CreateClientPush(44, 99)),
            Throws.TypeOf<InvalidOperationException>());

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(RoomClientRpcDispatchResult.Applied));
            Assert.That(receivedValue, Is.EqualTo(9));
            Assert.That(unavailable, Is.EqualTo(RoomClientRpcDispatchResult.EntityUnavailable));
            Assert.That(center.TryHandle(new RoomPushHead(ushort.MaxValue, default)),
                Is.EqualTo(RoomClientRpcDispatchResult.Unknown));
            Assert.That(world.HasBaseline, Is.True);
            Assert.That(world.WorldRevision, Is.EqualTo(1));
        });
    }

    [Test]
    public void RoomSyncSendsFullOrDiffBeforeQueuedRpc()
    {
        var connections = new RoomConnectionRegistry();
        var pushHub = new RoomPushHub();
        var state = new Game001RoomState("rpc-order-room", connections, pushHub);
        var syncSystem = new RoomSyncSystem(pushHub, state);
        int connectionId = connections.Add(101, state.RoomId);
        state.ActiveConnectionIds.Add(connectionId);
        state.PendingFullStateConnections.Add(connectionId);
        var hashes = new List<ushort>();
        pushHub.Register(connectionId, push => hashes.Add(push.PushHash));
        Entity first = state.Entities.CreateEntity(new TestPositionComponent { X = 1 });
        state.RpcQueue.SendObservers<TestClientRpcMessage, TestPositionComponent>(
            new TestClientRpcMessage { EntityId = first.Id, Value = 1 },
            true);

        syncSystem.Update(0, 1, 20);

        Assert.That(hashes, Is.EqualTo(new[]
        {
            TypeId<Game001.Core.RoomFullStatePush>.stableId16,
            TypeId<TestClientRpcMessage>.stableId16,
        }));

        hashes.Clear();
        Entity second = state.Entities.CreateEntity(new TestPositionComponent { X = 2 });
        state.RpcQueue.SendObservers<TestClientRpcMessage, TestPositionComponent>(
            new TestClientRpcMessage { EntityId = second.Id, Value = 2 },
            true);

        syncSystem.Update(20, 2, 40);

        Assert.That(hashes, Is.EqualTo(new[]
        {
            TypeId<Game001.Core.RoomDiffStatePush>.stableId16,
            TypeId<TestClientRpcMessage>.stableId16,
        }));
        state.Destroy();
    }

    private static void Send(RoomCommandServerCenter center, int connectionId, int entityId, int value)
    {
        byte[] payload = MemoryPackSerializer.Serialize(new TestServerRpcMessage
        {
            EntityId = entityId,
            Value = value,
        });
        center.TryHandle(connectionId, new RoomCommandHead(
            TypeId<TestServerRpcMessage>.stableId16,
            new ArraySegment<byte>(payload)));
    }

    private static RoomPushHead CreateClientPush(int entityId, int value)
    {
        byte[] payload = MemoryPackSerializer.Serialize(new TestClientRpcMessage
        {
            EntityId = entityId,
            Value = value,
        });
        return new RoomPushHead(
            TypeId<TestClientRpcMessage>.stableId16,
            new ArraySegment<byte>(payload));
    }

    private static int ReadClientValue(RoomPushHead push)
    {
        return MemoryPackSerializer.Deserialize<TestClientRpcMessage>(push.Payload).Value;
    }
}

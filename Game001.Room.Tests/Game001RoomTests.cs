using Game001.Core;
using Game001.Core.Ecs;
using Game001.Room.Runtime;
using Game001.Room.Systems;
using Friflo.Engine.ECS;
using GameServer.Core.Rooms;
using MemoryPack;
using UnityToolkit;
using Xunit;

namespace Game001.Room.Tests;

public sealed class Game001RoomTests
{
    private const string RoomId = "room-test";
    private const long Uid = 1001;

    [Fact]
    public void CreateRoomAddsCreatorAndSendsFullState()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomFullStatePush> pushes);
        RoomLifecycleSystem lifecycleSystem = GetLifecycleSystem(room);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, string.Empty);

        string message = lifecycleSystem.CreateRoom(connectionId, Uid);
        room.Update(12345, 1);

        Assert.Contains("created room=room-test", message, StringComparison.Ordinal);
        Assert.Single(room.State.Players);
        Assert.Contains(Uid, room.State.Players);
        Assert.True(room.State.TryGetPlayerEntity(Uid, out Entity creatorEntity));
        Assert.True(creatorEntity.HasComponent<RoomPlayerComponent>());
        RoomFullStatePush push = Assert.Single(pushes);
        Assert.Equal(RoomId, push.Room.RoomId);
        Assert.Equal(1, push.Room.PlayerCount);
        Assert.Equal(new[] { Uid }, push.Players);
        Assert.Empty(push.DisconnectedPlayers);
        EcsEntitySnapshot entitySnapshot = Assert.Single(push.Entities);
        EcsComponentSnapshot playerComponent = Assert.Single(entitySnapshot.Components);
        Assert.Equal(TypeId<RoomPlayerComponent>.stableId16, playerComponent.ComponentTypeId);
        RoomPlayerComponent player = MemoryPackSerializer.Deserialize<RoomPlayerComponent>(playerComponent.Payload);
        Assert.Equal(Uid, player.Uid);
    }

    [Fact]
    public void JoinRoomSendsFullStateToJoinedConnection()
    {
        const long otherUid = 1002;
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomFullStatePush> pushes);
        RoomLifecycleSystem lifecycleSystem = GetLifecycleSystem(room);
        int otherConnectionId = AddConnection(connections, pushHub, new List<RoomFullStatePush>(), otherUid, RoomId);
        lifecycleSystem.JoinRoom(otherConnectionId, otherUid);
        room.Update(12345, 1);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);

        string first = lifecycleSystem.JoinRoom(connectionId, Uid);
        room.Update(12378, 2);
        string second = lifecycleSystem.JoinRoom(connectionId, Uid);
        room.Update(12411, 3);

        Assert.Contains("players=2", first, StringComparison.Ordinal);
        Assert.Contains("players=2", second, StringComparison.Ordinal);
        Assert.Equal(2, room.State.Players.Count);
        Assert.True(room.State.TryGetPlayerEntity(Uid, out _));
        Assert.True(room.State.TryGetPlayerEntity(otherUid, out _));
        Assert.Equal(2, pushes.Count);
        RoomFullStatePush push = pushes[0];
        Assert.Equal(RoomId, push.Room.RoomId);
        Assert.Equal(2, push.Room.PlayerCount);
        Assert.Contains(Uid, push.Players);
        Assert.Contains(otherUid, push.Players);
    }

    [Fact]
    public void LeaveRoomRemovesPlayerAndDisconnectedState()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomFullStatePush> pushes);
        RoomLifecycleSystem lifecycleSystem = GetLifecycleSystem(room);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);
        lifecycleSystem.JoinRoom(connectionId, Uid);
        room.Update(12345, 1);
        lifecycleSystem.DisconnectRoom(connectionId, Uid);
        room.Update(12378, 2);
        pushes.Clear();

        string message = lifecycleSystem.LeaveRoom(connectionId, Uid);
        room.Update(12411, 3);

        Assert.Contains("players=0", message, StringComparison.Ordinal);
        Assert.DoesNotContain(Uid, room.State.Players);
        Assert.DoesNotContain(Uid, room.State.DisconnectedPlayers);
        Assert.False(room.State.TryGetPlayerEntity(Uid, out _));
        Assert.Empty(pushes);
    }

    [Fact]
    public void DisconnectOnlyMarksPlayersInRoom()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomFullStatePush> pushes);
        RoomLifecycleSystem lifecycleSystem = GetLifecycleSystem(room);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);

        lifecycleSystem.DisconnectRoom(connectionId, Uid);
        room.Update(12345, 1);

        Assert.Empty(room.State.DisconnectedPlayers);
        Assert.Empty(pushes);

        lifecycleSystem.JoinRoom(connectionId, Uid);
        room.Update(12378, 2);
        pushes.Clear();

        lifecycleSystem.DisconnectRoom(connectionId, Uid);
        room.Update(12411, 3);

        Assert.Contains(Uid, room.State.DisconnectedPlayers);
        Assert.True(room.State.TryGetPlayerEntity(Uid, out Entity playerEntity));
        Assert.True(playerEntity.TryGetComponent(out RoomDisconnectedComponent disconnected));
        Assert.True(disconnected.TimeMs > 0);
        Assert.Empty(pushes);
    }

    [Fact]
    public void DisconnectSendsRoomDiffStatePushToOtherConnections()
    {
        const long otherUid = 1002;
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomFullStatePush> pushes);
        RoomLifecycleSystem lifecycleSystem = GetLifecycleSystem(room);
        var diffs = new List<RoomDiffStatePush>();
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);
        lifecycleSystem.CreateRoom(connectionId, Uid);
        room.Update(12345, 1);

        int otherConnectionId = AddConnection(connections, pushHub, new List<RoomFullStatePush>(), otherUid, RoomId, diffs);
        lifecycleSystem.JoinRoom(otherConnectionId, otherUid);
        room.Update(12378, 2);
        diffs.Clear();

        lifecycleSystem.DisconnectRoom(connectionId, Uid);
        room.Update(12411, 3);

        RoomDiffStatePush diff = Assert.Single(diffs);
        Assert.Equal(2, diff.SourceFrame);
        Assert.Equal(3, diff.TargetFrame);
        EcsComponentChange componentChange = Assert.Single(
            diff.ComponentChanges,
            change => change.ComponentTypeId == TypeId<RoomDisconnectedComponent>.stableId16);
        Assert.Equal(EcsChangeKind.Add, componentChange.Kind);
        RoomDisconnectedComponent disconnected = MemoryPackSerializer.Deserialize<RoomDisconnectedComponent>(componentChange.Payload);
        Assert.True(disconnected.TimeMs > 0);
    }

    [Fact]
    public void DisconnectDiffKeepsComponentChangeOrder()
    {
        const long secondUid = 1002;
        const long observerUid = 1003;
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomFullStatePush> pushes);
        RoomLifecycleSystem lifecycleSystem = GetLifecycleSystem(room);
        var diffs = new List<RoomDiffStatePush>();
        int firstConnectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);
        lifecycleSystem.CreateRoom(firstConnectionId, Uid);
        room.Update(12345, 1);

        int secondConnectionId = AddConnection(connections, pushHub, new List<RoomFullStatePush>(), secondUid, RoomId);
        lifecycleSystem.JoinRoom(secondConnectionId, secondUid);
        room.Update(12378, 2);

        int observerConnectionId = AddConnection(connections, pushHub, new List<RoomFullStatePush>(), observerUid, RoomId, diffs);
        lifecycleSystem.JoinRoom(observerConnectionId, observerUid);
        room.Update(12411, 3);
        diffs.Clear();

        Assert.True(room.State.TryGetPlayerEntity(Uid, out Entity firstEntity));
        Assert.True(room.State.TryGetPlayerEntity(secondUid, out Entity secondEntity));

        lifecycleSystem.DisconnectRoom(firstConnectionId, Uid);
        lifecycleSystem.DisconnectRoom(secondConnectionId, secondUid);
        room.Update(12444, 4);

        RoomDiffStatePush diff = Assert.Single(diffs);
        EcsComponentChange[] disconnectedChanges = diff.ComponentChanges
            .Where(change => change.ComponentTypeId == TypeId<RoomDisconnectedComponent>.stableId16)
            .ToArray();
        Assert.Equal(2, disconnectedChanges.Length);
        Assert.Equal((int)firstEntity.Pid, disconnectedChanges[0].EntityId);
        Assert.Equal((int)secondEntity.Pid, disconnectedChanges[1].EntityId);
    }

    [Fact]
    public void PingRoomReturnsPong()
    {
        Game001Room room = CreateRoom(out _, out _, out _);
        RoomLifecycleSystem lifecycleSystem = GetLifecycleSystem(room);

        string message = lifecycleSystem.PingRoom(Uid);

        Assert.Contains("pong uid=1001", message, StringComparison.Ordinal);
    }

    [Fact]
    public void UpdateSetsFrameAndTime()
    {
        Game001Room room = CreateRoom(out _, out _, out _);

        room.Update(12345, 9);

        Assert.Equal(9, room.State.Frame);
        Assert.Equal(12345, room.State.LastUpdateTimeMs);
    }

    [Fact]
    public void RoomCreatesFrifloSystemRunner()
    {
        Game001Room room = CreateRoom(out _, out _, out _);

        Assert.Same(room.State.Entities, room.World);
        Assert.Same(room.State.EcsSystems, room.EcsSystems);
        Assert.True(room.Systems.TryGetSystem(out FrifloSystemRunnerSystem? runner));
        Assert.Same(room.EcsSystems, runner.Root);
    }

    private static Game001Room CreateRoom(
        out RoomConnectionRegistry connections,
        out RoomPushHub pushHub,
        out List<RoomFullStatePush> pushes)
    {
        connections = new RoomConnectionRegistry();
        pushHub = new RoomPushHub();
        pushes = new List<RoomFullStatePush>();
        return new Game001Room(RoomId, pushHub);
    }

    private static RoomLifecycleSystem GetLifecycleSystem(Game001Room room)
    {
        if (!room.Systems.TryGetSystem(out RoomLifecycleSystem? lifecycleSystem))
        {
            throw new InvalidOperationException("missing RoomLifecycleSystem");
        }

        return lifecycleSystem;
    }

    private static int AddConnection(
        RoomConnectionRegistry connections,
        RoomPushHub pushHub,
        List<RoomFullStatePush> pushes,
        long uid,
        string roomId,
        List<RoomDiffStatePush>? diffs = null)
    {
        int connectionId = connections.Add(uid, roomId);
        pushHub.Register(connectionId, push =>
        {
            if (push.PushHash == TypeId<RoomFullStatePush>.stableId16)
            {
                pushes.Add(MemoryPackSerializer.Deserialize<RoomFullStatePush>(push.Payload));
            }

            if (diffs != null && push.PushHash == TypeId<RoomDiffStatePush>.stableId16)
            {
                diffs.Add(MemoryPackSerializer.Deserialize<RoomDiffStatePush>(push.Payload));
            }
        });

        return connectionId;
    }
}

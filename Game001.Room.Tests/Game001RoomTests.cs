using Game001.Core;
using Game001.Room.Runtime;
using Game001.Room.Systems;
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
        RoomFullStatePush push = Assert.Single(pushes);
        Assert.Equal(RoomId, push.Room.RoomId);
        Assert.Equal(1, push.Room.PlayerCount);
        Assert.Equal(new[] { Uid }, push.Players);
        Assert.Empty(push.DisconnectedPlayers);
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
        Assert.Empty(pushes);
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
        string roomId)
    {
        int connectionId = connections.Add(uid, roomId);
        pushHub.Register(connectionId, push =>
        {
            if (push.PushHash == TypeId<RoomFullStatePush>.stableId16)
            {
                pushes.Add(MemoryPackSerializer.Deserialize<RoomFullStatePush>(push.Payload));
            }
        });

        return connectionId;
    }
}

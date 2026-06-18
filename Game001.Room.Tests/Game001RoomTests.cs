using Game001.Core;
using Game001.Room.Runtime;
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
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, string.Empty);

        string message = room.CreateRoom(connectionId, Uid);

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
        int otherConnectionId = AddConnection(connections, pushHub, new List<RoomFullStatePush>(), otherUid, RoomId);
        room.JoinRoom(otherConnectionId, otherUid);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);

        string first = room.JoinRoom(connectionId, Uid);
        string second = room.JoinRoom(connectionId, Uid);

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
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);
        room.JoinRoom(connectionId, Uid);
        room.DisconnectRoom(connectionId, Uid);
        pushes.Clear();

        string message = room.LeaveRoom(connectionId, Uid);

        Assert.Contains("players=0", message, StringComparison.Ordinal);
        Assert.DoesNotContain(Uid, room.State.Players);
        Assert.DoesNotContain(Uid, room.State.DisconnectedPlayers);
        Assert.Empty(pushes);
    }

    [Fact]
    public void DisconnectOnlyMarksPlayersInRoom()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomFullStatePush> pushes);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);

        room.DisconnectRoom(connectionId, Uid);

        Assert.Empty(room.State.DisconnectedPlayers);
        Assert.Empty(pushes);

        room.JoinRoom(connectionId, Uid);
        pushes.Clear();

        room.DisconnectRoom(connectionId, Uid);

        Assert.Contains(Uid, room.State.DisconnectedPlayers);
        Assert.Empty(pushes);
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

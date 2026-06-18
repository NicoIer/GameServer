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
    public void CreateRoomAddsCreatorAndSendsRoomPush()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomEventPush> pushes);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, string.Empty);

        string message = room.CreateRoom(connectionId, Uid);

        Assert.Contains("created room=room-test", message, StringComparison.Ordinal);
        Assert.Single(room.State.Players);
        Assert.Contains(Uid, room.State.Players);
        RoomEventPush push = Assert.Single(pushes);
        Assert.Equal(RoomPushType.RoomCreated, push.Type);
        Assert.Equal(Uid, push.Uid);
        Assert.Equal(RoomId, push.Room.RoomId);
        Assert.Equal(1, push.Room.PlayerCount);
    }

    [Fact]
    public void JoinRoomIsIdempotentForPlayerCount()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomEventPush> pushes);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);

        string first = room.JoinRoom(connectionId, Uid);
        string second = room.JoinRoom(connectionId, Uid);

        Assert.Contains("players=1", first, StringComparison.Ordinal);
        Assert.Contains("players=1", second, StringComparison.Ordinal);
        Assert.Single(room.State.Players);
        RoomEventPush push = Assert.Single(pushes);
        Assert.Equal(RoomPushType.PlayerJoined, push.Type);
    }

    [Fact]
    public void LeaveRoomRemovesPlayerAndDisconnectedState()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomEventPush> pushes);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);
        room.JoinRoom(connectionId, Uid);
        room.DisconnectRoom(connectionId, Uid);
        pushes.Clear();

        string message = room.LeaveRoom(connectionId, Uid);

        Assert.Contains("players=0", message, StringComparison.Ordinal);
        Assert.DoesNotContain(Uid, room.State.Players);
        Assert.DoesNotContain(Uid, room.State.DisconnectedPlayers);
        RoomEventPush push = Assert.Single(pushes);
        Assert.Equal(RoomPushType.PlayerLeft, push.Type);
    }

    [Fact]
    public void DisconnectOnlyMarksPlayersInRoom()
    {
        Game001Room room = CreateRoom(out RoomConnectionRegistry connections, out RoomPushHub pushHub, out List<RoomEventPush> pushes);
        int connectionId = AddConnection(connections, pushHub, pushes, Uid, RoomId);

        room.DisconnectRoom(connectionId, Uid);

        Assert.Empty(room.State.DisconnectedPlayers);
        Assert.Empty(pushes);

        room.JoinRoom(connectionId, Uid);
        pushes.Clear();

        room.DisconnectRoom(connectionId, Uid);

        Assert.Contains(Uid, room.State.DisconnectedPlayers);
        RoomEventPush push = Assert.Single(pushes);
        Assert.Equal(RoomPushType.PlayerDisconnected, push.Type);
    }

    [Fact]
    public void UpdateAdvancesFrameAndTime()
    {
        Game001Room room = CreateRoom(out _, out _, out _);

        room.Update(12345, 9);

        Assert.Equal(9, room.State.Frame);
        Assert.Equal(12345, room.State.LastUpdateTimeMs);
    }

    private static Game001Room CreateRoom(
        out RoomConnectionRegistry connections,
        out RoomPushHub pushHub,
        out List<RoomEventPush> pushes)
    {
        connections = new RoomConnectionRegistry();
        pushHub = new RoomPushHub();
        pushes = new List<RoomEventPush>();
        return new Game001Room(RoomId, connections, pushHub);
    }

    private static int AddConnection(
        RoomConnectionRegistry connections,
        RoomPushHub pushHub,
        List<RoomEventPush> pushes,
        long uid,
        string roomId)
    {
        int connectionId = connections.Add(uid, roomId);
        pushHub.Register(connectionId, push =>
        {
            if (push.PushHash == TypeId<RoomEventPush>.stableId16)
            {
                pushes.Add(MemoryPackSerializer.Deserialize<RoomEventPush>(push.Payload));
            }
        });

        return connectionId;
    }
}

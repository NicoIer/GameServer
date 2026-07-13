using Game001.Core;
using GameServer.Core.Rooms;
using MemoryPack;
using UnityToolkit;

namespace GameServer.Core.Tests;

[TestFixture]
public sealed class RoomCommandTests
{
    [Test]
    public void CommandHeadRoundTripPreservesPositionPayload()
    {
        var command = new UploadPositionCommand
        {
            CharacterEntityId = 42,
            Sequence = 7,
            ClientTimeMs = 12345,
            PositionX = 1.25f,
            PositionY = -2.5f,
        };
        byte[] payload = MemoryPackSerializer.Serialize(command);
        var head = new RoomCommandHead(
            TypeId<UploadPositionCommand>.stableId16,
            new ArraySegment<byte>(payload));

        byte[] bytes = MemoryPackSerializer.Serialize(head);
        RoomCommandHead copy = MemoryPackSerializer.Deserialize<RoomCommandHead>(bytes);
        UploadPositionCommand copiedCommand = MemoryPackSerializer.Deserialize<UploadPositionCommand>(copy.Payload);

        Assert.That(copy.CommandHash, Is.EqualTo(TypeId<UploadPositionCommand>.stableId16));
        Assert.That(copiedCommand.CharacterEntityId, Is.EqualTo(command.CharacterEntityId));
        Assert.That(copiedCommand.Sequence, Is.EqualTo(command.Sequence));
        Assert.That(copiedCommand.ClientTimeMs, Is.EqualTo(command.ClientTimeMs));
        Assert.That(copiedCommand.PositionX, Is.EqualTo(command.PositionX));
        Assert.That(copiedCommand.PositionY, Is.EqualTo(command.PositionY));
    }

    [Test]
    public void RegisteredCommandIsDispatchedWithConnectionId()
    {
        var center = new RoomCommandServerCenter();
        int handledConnectionId = 0;
        UploadPositionCommand handledCommand = default;
        center.Register<UploadPositionCommand>((connectionId, command) =>
        {
            handledConnectionId = connectionId;
            handledCommand = command;
        });
        var command = new UploadPositionCommand
        {
            CharacterEntityId = 11,
            Sequence = 3,
        };
        byte[] payload = MemoryPackSerializer.Serialize(command);
        var head = new RoomCommandHead(
            TypeId<UploadPositionCommand>.stableId16,
            new ArraySegment<byte>(payload));

        bool handled = center.TryHandle(19, head);

        Assert.That(handled, Is.True);
        Assert.That(handledConnectionId, Is.EqualTo(19));
        Assert.That(handledCommand.CharacterEntityId, Is.EqualTo(11));
        Assert.That(handledCommand.Sequence, Is.EqualTo(3));
    }

    [Test]
    public void UnknownCommandIsNotDispatched()
    {
        var center = new RoomCommandServerCenter();
        var head = new RoomCommandHead(ushort.MaxValue, default);

        Assert.That(center.TryHandle(1, head), Is.False);
    }

    [Test]
    public void InvalidPayloadDoesNotInvokeHandler()
    {
        var center = new RoomCommandServerCenter();
        bool invoked = false;
        center.Register<UploadPositionCommand>((_, _) => invoked = true);
        var head = new RoomCommandHead(
            TypeId<UploadPositionCommand>.stableId16,
            new ArraySegment<byte>(Array.Empty<byte>()));

        Assert.That(() => center.TryHandle(1, head), Throws.Exception);
        Assert.That(invoked, Is.False);
    }

    [Test]
    public void DuplicateCommandRegistrationFails()
    {
        var center = new RoomCommandServerCenter();
        center.Register<UploadPositionCommand>((_, _) => { });

        Assert.That(
            () => center.Register<UploadPositionCommand>((_, _) => { }),
            Throws.TypeOf<ArgumentException>());
    }
}

using System.Buffers.Binary;
using MemoryPack;

namespace GameServer.Core.Network;

public static class GameTcpFrame
{
    private const int HeaderSize = 4;
    private const int MaxFrameSize = 4 * 1024 * 1024;

    public static async Task<GamePacket?> ReadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        byte[] header = new byte[HeaderSize];
        bool hasHeader = await ReadExactOrEndAsync(stream, header, cancellationToken);
        if (!hasHeader)
        {
            return null;
        }

        int length = BinaryPrimitives.ReadInt32BigEndian(header);
        if (length <= 0 || length > MaxFrameSize)
        {
            throw new InvalidDataException($"invalid frame length={length}");
        }

        byte[] data = new byte[length];
        await ReadExactAsync(stream, data, cancellationToken);
        return MemoryPackSerializer.Deserialize<GamePacket>(data);
    }

    public static async Task WriteAsync(Stream stream, GamePacket packet, CancellationToken cancellationToken = default)
    {
        byte[] data = MemoryPackSerializer.Serialize(packet);
        await WriteRawAsync(stream, data, cancellationToken);
    }

    public static async Task WriteRawAsync(Stream stream, byte[] data, CancellationToken cancellationToken = default)
    {
        byte[] header = new byte[HeaderSize];
        BinaryPrimitives.WriteInt32BigEndian(header, data.Length);
        await stream.WriteAsync(header, cancellationToken);
        await stream.WriteAsync(data, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    private static async Task<bool> ReadExactOrEndAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken);
            if (read == 0)
            {
                if (offset != 0)
                {
                    throw new EndOfStreamException();
                }

                return false;
            }

            offset += read;
        }

        return true;
    }

    private static async Task ReadExactAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken);
            if (read == 0)
            {
                throw new EndOfStreamException();
            }

            offset += read;
        }
    }
}

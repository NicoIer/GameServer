using System.Diagnostics;
using MemoryPack;
using Network;
using Newtonsoft.Json;

namespace ToolkitTest;

[MemoryPackable]
public partial struct RandomNumberMessage : INetworkMessage
{
    public int number;
}

public class NetworkTests
{
    private NetworkBufferPool _bufferPool;

    [SetUp]
    public void Setup()
    {
        _bufferPool = new NetworkBufferPool(16);
    }

    [Test]
    public void Test1()
    {
        var random = new System.Random();
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            NetworkBuffer payloadBuffer = _bufferPool.Get();
            NetworkBuffer packetBuffer = _bufferPool.Get();
            var number = random.Next();
            RandomNumberMessage origin = new RandomNumberMessage { number = number };
            NetworkPacker.Pack(origin, payloadBuffer, packetBuffer);
            NetworkPacker.Unpack(packetBuffer, out var packet);
            Assert.IsTrue(packet.id == NetworkId<RandomNumberMessage>.Value);
            var unpacked = MemoryPackSerializer.Deserialize<RandomNumberMessage>(packet.payload);
            Assert.IsTrue(unpacked.number == number);
            _bufferPool.Return(payloadBuffer);
            _bufferPool.Return(packetBuffer);
        }

        stopwatch.Stop();
        TestContext.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}ms");
    }
}
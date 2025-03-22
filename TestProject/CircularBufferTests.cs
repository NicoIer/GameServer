using System.Numerics;
using GameCore.Jolt;
using MemoryPack;
using UnityToolkit;

namespace TestProject;

public class CircularBufferTests
{
    [Test]
    public void ShapeDataTest()
    {
        CircularBuffer<WorldData> snapshotBuffer = new CircularBuffer<WorldData>(10);
        for (int i = 0; i < 10; i++)
        {
            snapshotBuffer.PushBack(new WorldData()
            {
                frameCount = i
            });
        }

        Assert.That(snapshotBuffer.Count, Is.EqualTo(10));
        snapshotBuffer.PushBack(new WorldData() { frameCount = 11 });
        Assert.That(snapshotBuffer.Count, Is.EqualTo(10));
        Assert.That(snapshotBuffer[0].frameCount, Is.EqualTo(1));
        Assert.That(snapshotBuffer[9].frameCount, Is.EqualTo(11));
        
    }
}
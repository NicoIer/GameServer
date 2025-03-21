using System.Numerics;
using GameCore.Jolt;
using MemoryPack;

namespace TestProject;

public class JoltSerializeTests
{
    [Test]
    public void BoundingBox()
    {
        BoundingBox boundingBox = new BoundingBox();
        var result = MemoryPackSerializer.Serialize(boundingBox);
        // boundingBox只有2个Vector3 -> 6个float -> 24个byte 
        int count = sizeof(float) * 3 * 2 / sizeof(byte);
        Assert.That(result.Length, Is.EqualTo(count));
    }
}
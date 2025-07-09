using System.Numerics;
using GameCore.Physics;
using MemoryPack;

namespace TestProject;

public class JoltSerializeTests
{
    [Test]
    public void ShapeDataTest()
    {
        ShapeDataPacket.RegisterAll();
        BoxShapeData shapeData = new BoxShapeData(Vector3.One);
        ShapeDataPacket.Create(shapeData, out ShapeDataPacket data);
        IShapeData reverted = ShapeDataPacket.Deserialize(data);
        Assert.That(reverted, Is.TypeOf<BoxShapeData>());
        Assert.That(((BoxShapeData)reverted).halfExtents, Is.EqualTo(Vector3.One));
    }

    [Test]
    public void SerializeTest()
    {
        uint? nullObj = null;
        uint uintObj = 0;

        var s1 = MemoryPackSerializer.Serialize(nullObj);
        TestContext.Out.WriteLine(s1.Length);

        var s2 = MemoryPackSerializer.Serialize(uintObj);
        TestContext.Out.WriteLine(s2.Length);
    }
}
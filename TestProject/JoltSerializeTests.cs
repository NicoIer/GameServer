using System.Numerics;
using GameCore.Jolt;
using MemoryPack;

namespace TestProject;

public class JoltSerializeTests
{
    [Test]
    public void ShapeDataTest()
    {
        ShapeData.RegisterAll();
        BoxShapeData shapeData = new BoxShapeData(Vector3.One);
        ShapeData.Create(shapeData, out ShapeData data);
        IShapeData reverted = ShapeData.Revert(data);
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
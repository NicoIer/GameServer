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
        IShapeData reverted = data.Revert();
        Assert.That(reverted, Is.TypeOf<BoxShapeData>());
        Assert.That(((BoxShapeData)reverted).halfExtents, Is.EqualTo(Vector3.One));
        
    }
}
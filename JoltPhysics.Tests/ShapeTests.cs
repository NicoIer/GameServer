using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class ShapeTests : JoltTestBase
{
    [Test]
    public void SphereShape_Create_IsNotDisposed()
    {
        using var shape = new SphereShape(1.0f);
        Assert.That(shape.IsDisposed, Is.False);
    }

    [Test]
    public void BoxShape_Create_IsNotDisposed()
    {
        using var shape = new BoxShape(new Float3(1f, 1f, 1f));
        Assert.That(shape.IsDisposed, Is.False);
    }

    [Test]
    public void CapsuleShape_Create_IsNotDisposed()
    {
        using var shape = new CapsuleShape(1.0f, 0.5f);
        Assert.That(shape.IsDisposed, Is.False);
    }

    [Test]
    public void ScaledShape_Create_IsNotDisposed()
    {
        using var inner = new SphereShape(1.0f);
        using var shape = new ScaledShape(inner, new Float3(2f, 2f, 2f));
        Assert.That(shape.IsDisposed, Is.False);
    }

    [Test]
    public void ConvexHullShape_Create_IsNotDisposed()
    {
        var points = new Float3[]
        {
            new(0, 0, 0), new(1, 0, 0), new(0, 1, 0), new(0, 0, 1),
        };
        using var settings = new ConvexHullShapeSettings(points);
        using var shape = settings.CreateShape();
        Assert.That(shape.IsDisposed, Is.False);
        Assert.That(shape.NumPoints, Is.GreaterThanOrEqualTo(4u));
    }
}

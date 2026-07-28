using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class MathTests
{
    [Test]
    public void Float3_Constructor()
    {
        var v = new Float3(1f, 2f, 3f);
        Assert.That(v.x, Is.EqualTo(1f));
        Assert.That(v.y, Is.EqualTo(2f));
        Assert.That(v.z, Is.EqualTo(3f));
    }

    [Test]
    public void Float3_Zero()
    {
        var v = Float3.Zero;
        Assert.That(v.x, Is.EqualTo(0f));
        Assert.That(v.y, Is.EqualTo(0f));
        Assert.That(v.z, Is.EqualTo(0f));
    }

    [Test]
    public void Float3_Equality()
    {
        var a = new Float3(1f, 2f, 3f);
        var b = new Float3(1f, 2f, 3f);
        Assert.That(a, Is.EqualTo(b));
        Assert.That(a == b, Is.True);
    }

    [Test]
    public void Quat_Identity()
    {
        var q = Quat.Identity;
        Assert.That(q.x, Is.EqualTo(0f));
        Assert.That(q.y, Is.EqualTo(0f));
        Assert.That(q.z, Is.EqualTo(0f));
        Assert.That(q.w, Is.EqualTo(1f));
    }

    [Test]
    public void ObjectLayer_ImplicitConversion()
    {
        ObjectLayer layer = 5u;
        Assert.That(layer.Value, Is.EqualTo(5u));

        uint raw = layer;
        Assert.That(raw, Is.EqualTo(5u));
    }

    [Test]
    public void ObjectLayer_IntConversion()
    {
        ObjectLayer layer = 3;
        int val = layer;
        Assert.That(val, Is.EqualTo(3));
    }

    [Test]
    public void ObjectLayer_Equality()
    {
        ObjectLayer a = 7u;
        ObjectLayer b = 7u;
        Assert.That(a == b, Is.True);
        Assert.That(a != new ObjectLayer(8), Is.True);
    }

    [Test]
    public void BroadPhaseLayer_ImplicitConversion()
    {
        BroadPhaseLayer layer = (byte)2;
        Assert.That(layer.Value, Is.EqualTo(2));

        byte raw = layer;
        Assert.That(raw, Is.EqualTo(2));
    }

    [Test]
    public void BroadPhaseLayer_Equality()
    {
        BroadPhaseLayer a = (byte)1;
        BroadPhaseLayer b = (byte)1;
        Assert.That(a == b, Is.True);
    }

    [Test]
    public void CollisionGroup_Constructor()
    {
        var g = new CollisionGroup(10, 20);
        Assert.That(g.groupID, Is.EqualTo(10u));
        Assert.That(g.subGroupID, Is.EqualTo(20u));
    }

    [Test]
    public void BodyID_Invalid()
    {
        Assert.That(BodyID.Invalid.IsValid, Is.False);
    }

    [Test]
    public void BodyID_ValidCreation()
    {
        var id = new BodyID(42);
        Assert.That(id.IsValid, Is.True);
        Assert.That(id.Value, Is.EqualTo(42u));
    }
}

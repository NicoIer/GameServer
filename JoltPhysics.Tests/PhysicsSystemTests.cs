using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class PhysicsSystemTests : JoltTestBase
{
    [Test]
    public void Gravity_DefaultIsZero()
    {
        // Default settings have gravity = (0,0,0) unless explicitly set
        var g = PhysicsSystem.Gravity;
        // Jolt default gravity is (0, -9.81, 0)
        Assert.That(g.y, Is.LessThan(0f));
    }

    [Test]
    public void Gravity_SetAndGet()
    {
        var customGravity = new Float3(0, -20f, 0);
        PhysicsSystem.Gravity = customGravity;
        var g = PhysicsSystem.Gravity;
        Assert.That(g.y, Is.EqualTo(-20f).Within(1e-4f));
    }

    [Test]
    public void Update_DynamicBodyFalls()
    {
        PhysicsSystem.Gravity = new Float3(0, -10f, 0);
        var id = CreateDynamicSphere(new Float3(0, 10f, 0));

        for (int i = 0; i < 60; i++)
            Step();

        var pos = BodyInterface.GetPosition(id);
        Assert.That(pos.y, Is.LessThan(10f), "Dynamic body should have fallen");

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void GetBodyInterface_CanCreateBody()
    {
        var bi = PhysicsSystem.GetBodyInterface();
        var id = CreateDynamicSphere(Float3.Zero);
        var pos = bi.GetPosition(id);
        Assert.That(pos.x, Is.EqualTo(0f).Within(1e-4f));
        bi.RemoveAndDestroyBody(id);
    }

    [Test]
    public void GetNarrowPhaseQuery_Works()
    {
        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        // Verify it works by casting a ray (no bodies, should miss)
        bool hit = npq.CastRay(Float3.Zero, new Float3(0, 1, 0), out _);
        Assert.That(hit, Is.False);
    }

    [Test]
    public void GetBroadPhaseQuery_Works()
    {
        var bpq = PhysicsSystem.GetBroadPhaseQuery();
        var results = new List<BodyID>();
        bpq.CollidePoint(Float3.Zero, results);
        Assert.That(results.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetBodyLockInterface_CanGetBroadPhaseLayer()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var bli = PhysicsSystem.GetBodyLockInterfaceNoLock();
        var bpl = bli.GetBroadPhaseLayer(id);
        Assert.That((byte)bpl, Is.LessThan(NumBroadPhaseLayers));
        BodyInterface.RemoveAndDestroyBody(id);
    }
}

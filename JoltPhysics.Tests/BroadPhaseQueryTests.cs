using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class BroadPhaseQueryTests : JoltTestBase
{
    [Test]
    public void CastRay_HitsBody()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var bpq = PhysicsSystem.GetBroadPhaseQuery();
        var results = new List<BroadPhaseCastResult>();

        var origin = new Float3(0, 20f, 0);
        var direction = new Float3(0, -40f, 0);

        bool hit = bpq.CastRay(origin, direction, results);
        Assert.That(hit, Is.True);
        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CollideAABox_FindsOverlappingBodies()
    {
        var box1 = CreateStaticBox(Float3.Zero, new Float3(1f, 1f, 1f));
        var box2 = CreateStaticBox(new Float3(100f, 0, 0), new Float3(1f, 1f, 1f));
        Step();

        var bpq = PhysicsSystem.GetBroadPhaseQuery();
        var results = new List<BodyID>();

        // AABB around origin, should only find box1
        bpq.CollideAABox(new Float3(-5f, -5f, -5f), new Float3(5f, 5f, 5f), results);

        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0], Is.EqualTo(box1));

        BodyInterface.RemoveAndDestroyBody(box1);
        BodyInterface.RemoveAndDestroyBody(box2);
    }

    [Test]
    public void CollideSphere_FindsOverlappingBodies()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(1f, 1f, 1f));
        Step();

        var bpq = PhysicsSystem.GetBroadPhaseQuery();
        var results = new List<BodyID>();

        bpq.CollideSphere(Float3.Zero, 5f, results);

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CollidePoint_FindsContainingBodies()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var bpq = PhysicsSystem.GetBroadPhaseQuery();
        var results = new List<BodyID>();

        bpq.CollidePoint(Float3.Zero, results);

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }
}

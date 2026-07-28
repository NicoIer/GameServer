using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class NarrowPhaseQueryTests: JoltTestBase
{
    [Test]
    public void CastRay_ClosestHit_HitsStaticBox()
    {
        // Place a static box at (0,0,0)
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step(); // Ensure broadphase is updated

        var npq = PhysicsSystem.GetNarrowPhaseQuery();

        // Ray from (0, 20, 0) downward
        var origin = new Float3(0, 20f, 0);
        var direction = new Float3(0, -40f, 0); // Length = max distance

        bool hit = npq.CastRay(origin, direction, out var result);
        Assert.That(hit, Is.True);
        Assert.That(result.BodyID, Is.EqualTo(boxId));
        Assert.That(result.Fraction, Is.GreaterThan(0f).And.LessThan(1f));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CastRay_Miss_ReturnsFalse()
    {
        var boxId = CreateStaticBox(new Float3(100f, 0, 0), new Float3(1f, 1f, 1f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();

        // Ray going in opposite direction
        var origin = new Float3(-100f, 0, 0);
        var direction = new Float3(-10f, 0, 0);

        bool hit = npq.CastRay(origin, direction, out _);
        Assert.That(hit, Is.False);

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CastRay_CollectAll_ReturnsMultipleHits()
    {
        var box1 = CreateStaticBox(new Float3(0, 5f, 0), new Float3(5f, 0.5f, 5f));
        var box2 = CreateStaticBox(new Float3(0, -5f, 0), new Float3(5f, 0.5f, 5f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        var results = new List<RayCastResult>();

        var origin = new Float3(0, 20f, 0);
        var direction = new Float3(0, -40f, 0);

        npq.CastRay(origin, direction,
            RayCastSettings.Default,
            CollisionCollectorType.AllHit,
            results);

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(2));

        BodyInterface.RemoveAndDestroyBody(box1);
        BodyInterface.RemoveAndDestroyBody(box2);
    }

    [Test]
    public void CollidePoint_DetectsInside()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        var results = new List<CollidePointResult>();

        // Point inside the box
        npq.CollidePoint(Float3.Zero, CollisionCollectorType.AllHit, results);

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(results[0].BodyID, Is.EqualTo(boxId));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CollideShape_DetectsOverlap()
    {
        var boxId = CreateStaticBox(Float3.Zero, new Float3(5f, 5f, 5f));
        Step();

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        var results = new List<CollideShapeResult>();

        using var testShape = new SphereShape(1f);
        var settings = CollideShapeSettings.Default;
        npq.CollideShape(testShape,
            new Float3(1f, 1f, 1f),
            Mat4.Identity,
            Float3.Zero,
            results, settings,
            collectorType: CollisionCollectorType.AllHit);

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CastShape_DetectsHit()
    {
        var boxId = CreateStaticBox(new Float3(0, 0, 10f), new Float3(5f, 5f, 5f));
        WaitForBodyAddedAndReadyForQuery(boxId);

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        ShapeCastResult firstHit = default;
        var hitCount = 0;

        using var testShape = new SphereShape(0.5f);
        var settings = ShapeCastSettings.Default;
        bool hit = npq.CastShape(testShape,
            Mat4.Identity,
            new Float3(0, 0, 20f), // direction
            settings,
            Float3.Zero,
            (in ShapeCastResult result) =>
            {
                if (hitCount == 0)
                    firstHit = result;
                ++hitCount;
                return result.Fraction;
            });

        Assert.That(hit, Is.True);
        Assert.That(hitCount, Is.GreaterThanOrEqualTo(1));
        Assert.That(firstHit.Fraction, Is.GreaterThan(0f).And.LessThan(1f));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CastShape_CollectResults_DetectsHit()
    {
        var boxId = CreateStaticBox(new Float3(0, 0, 10f), new Float3(5f, 5f, 5f));
        WaitForBodyAddedAndReadyForQuery(boxId);

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        var results = new List<ShapeCastResult>();

        using var testShape = new SphereShape(0.5f);
        var settings = ShapeCastSettings.Default;
        npq.CastShape(testShape,
            Mat4.Identity,
            new Float3(0, 0, 20f), // direction
            settings,
            Float3.Zero,
            results,
            collectorType: CollisionCollectorType.AllHit);

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(results[0].Fraction, Is.GreaterThan(0f).And.LessThan(1f));

        BodyInterface.RemoveAndDestroyBody(boxId);
    }

    [Test]
    public void CastShape_DetectsDynamicBody()
    {
        var sphereId = CreateDynamicSphere(new Float3(0, 0, 10f), 1f);
        BodyInterface.SetGravityFactor(sphereId, 0f);
        BodyInterface.SetLinearVelocity(sphereId, Float3.Zero);
        WaitForBodyAddedAndReadyForQuery(sphereId);

        var npq = PhysicsSystem.GetNarrowPhaseQuery();
        var results = new List<ShapeCastResult>();

        using var testShape = new SphereShape(0.5f);
        var settings = ShapeCastSettings.Default;
        npq.CastShape(testShape,
            Mat4.Identity,
            new Float3(0, 0, 20f),
            settings,
            Float3.Zero,
            results,
            collectorType: CollisionCollectorType.AllHit);

        Assert.That(results.Count, Is.GreaterThanOrEqualTo(1));

        var hit = results.Find(result => result.BodyID2 == sphereId);
        Assert.That(hit.BodyID2, Is.EqualTo(sphereId));
        Assert.That(hit.Fraction, Is.GreaterThan(0f).And.LessThan(1f));

        BodyInterface.RemoveAndDestroyBody(sphereId);
    }
}
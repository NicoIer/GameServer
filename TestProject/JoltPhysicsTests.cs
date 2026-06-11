using System.Numerics;
using GameCore.Physics;
using JoltPhysicsSharp;
using JoltServer;

namespace TestProject;

public class JoltPhysicsTests
{

    [Test]
    public void NAN()
    {
        using var app = new JoltApplication();
        Assert.That(app.physicsWorld, Is.Not.Null);
    }

    [Test]
    public void CastShape_WhenSweptSphereHitsStaticBox_ReturnsClosestHit()
    {
        using var app = new JoltApplication();
        var physicsSystem = app.physicsWorld.physicsSystem;

        using var targetShape = new BoxShape(new Vector3(1.0f));
        using var targetSettings = new BodyCreationSettings(
            targetShape,
            new Vector3(5.0f, 0.0f, 0.0f),
            Quaternion.Identity,
            JoltPhysicsSharp.MotionType.Static,
            new ObjectLayer((uint)ObjectLayers.NonMoving));
        BodyID targetBody = physicsSystem.BodyInterface.CreateAndAddBody(
            targetSettings,
            JoltPhysicsSharp.Activation.DontActivate);

        try
        {
            physicsSystem.OptimizeBroadPhase();

            using var castShape = new SphereShape(0.5f);
            Matrix4x4 startTransform = Matrix4x4.CreateTranslation(Vector3.Zero);
            Vector3 castDirection = new(10.0f, 0.0f, 0.0f);
            List<ShapeCastResult> results = [];

            bool hit = physicsSystem.NarrowPhaseQuery.CastShape(
                castShape,
                startTransform,
                castDirection,
                Vector3.Zero,
                CollisionCollectorType.ClosestHit,
                results);

            Assert.That(hit, Is.True);
            Assert.That(results, Has.Count.EqualTo(1));

            ShapeCastResult result = results[0];
            Assert.That(result.BodyID2, Is.EqualTo(targetBody));
            // Assert.That(result.Fraction, Is.EqualTo(0.35f).Within(0.02f));
            // AssertApproximately(new Vector3(3.5f, 0.0f, 0.0f), result.ContactPointOn1, 0.05f);
            // AssertApproximately(new Vector3(4.0f, 0.0f, 0.0f), result.ContactPointOn2, 0.05f);
        }
        finally
        {
            physicsSystem.BodyInterface.RemoveAndDestroyBody(targetBody);
        }
    }

    private static void AssertApproximately(Vector3 expected, Vector3 actual, float tolerance)
    {
        Assert.That(actual.X, Is.EqualTo(expected.X).Within(tolerance));
        Assert.That(actual.Y, Is.EqualTo(expected.Y).Within(tolerance));
        Assert.That(actual.Z, Is.EqualTo(expected.Z).Within(tolerance));
    }
}

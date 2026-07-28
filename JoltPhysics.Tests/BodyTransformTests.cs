using System;
using System.Numerics;
using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class BodyTransformTests : JoltTestBase
{
    private const float Epsilon = 1e-4f;

    [Test]
    public void MakeTransform_IdentityRotation_WritesTranslationColumn()
    {
        var position = new Vector3(3.5f, -2.25f, 9.0f);

        var transform = JoltAPI.MakeTransform(Quaternion.Identity, position);

        AssertFloat4(transform.Column0, 1f, 0f, 0f, 0f);
        AssertFloat4(transform.Column1, 0f, 1f, 0f, 0f);
        AssertFloat4(transform.Column2, 0f, 0f, 1f, 0f);
        AssertFloat4(transform.Column3, position.X, position.Y, position.Z, 1f);
    }

    [Test]
    public void Body_WorldTransform_MatchesPositionRotationTransform()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var position = new Float3(4.0f, 5.5f, -6.25f);
        var rotation = CreateYRotation(MathF.PI * 0.5f);

        BodyInterface.SetPositionAndRotation(id, position, rotation, Activation.Activate);

        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();
        using (var lockRead = lockInterface.LockRead(id))
        {
            Assert.That(lockRead.Succeeded, Is.True);

            var body = lockRead.Body;
            var expected = JoltAPI.MakeTransform((Quaternion)rotation, (Vector3)position);

            AssertMat4(body.WorldTransform, expected);
            AssertTranslation(body.WorldTransform, position);
        }

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void Body_CenterOfMassTransform_MatchesWorldTransform_ForCenteredSphere()
    {
        var id = CreateDynamicSphere(new Float3(-2.0f, 3.0f, 7.5f));
        var rotation = CreateYRotation(MathF.PI / 3.0f);

        BodyInterface.SetRotation(id, rotation, Activation.Activate);

        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();
        using (var lockRead = lockInterface.LockRead(id))
        {
            Assert.That(lockRead.Succeeded, Is.True);

            var body = lockRead.Body;

            AssertMat4(body.CenterOfMassTransform, body.WorldTransform);
            AssertTranslation(body.CenterOfMassTransform, body.CenterOfMassPosition);
        }

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void Body_InverseCenterOfMassTransform_InvertsTranslation_ForIdentityRotation()
    {
        var position = new Float3(1.25f, -3.5f, 8.0f);
        var id = CreateDynamicSphere(position);

        BodyInterface.SetRotation(id, Quat.Identity, Activation.Activate);

        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();
        using (var lockRead = lockInterface.LockRead(id))
        {
            Assert.That(lockRead.Succeeded, Is.True);

            var inverse = lockRead.Body.InverseCenterOfMassTransform;

            AssertFloat4(inverse.Column0, 1f, 0f, 0f, 0f);
            AssertFloat4(inverse.Column1, 0f, 1f, 0f, 0f);
            AssertFloat4(inverse.Column2, 0f, 0f, 1f, 0f);
            AssertFloat4(inverse.Column3, -position.x, -position.y, -position.z, 1f);
        }

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void Body_InverseInertia_ReturnsFiniteMatrix()
    {
        var id = CreateDynamicSphere(Float3.Zero, 1.0f);

        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();
        using (var lockRead = lockInterface.LockRead(id))
        {
            Assert.That(lockRead.Succeeded, Is.True);

            AssertMat4IsFinite(lockRead.Body.InverseInertia);
        }

        BodyInterface.RemoveAndDestroyBody(id);
    }

    private static Quat CreateYRotation(float radians)
    {
        float half = radians * 0.5f;
        return new Quat(0f, MathF.Sin(half), 0f, MathF.Cos(half));
    }

    private static void AssertMat4(Mat4 actual, Mat4 expected)
    {
        AssertFloat4(actual.Column0, expected.Column0);
        AssertFloat4(actual.Column1, expected.Column1);
        AssertFloat4(actual.Column2, expected.Column2);
        AssertFloat4(actual.Column3, expected.Column3);
    }

    private static void AssertTranslation(Mat4 transform, Float3 expected)
    {
        AssertFloat4(transform.Column3, expected.x, expected.y, expected.z, 1f);
    }

    private static void AssertFloat4(Float4 actual, Float4 expected)
    {
        AssertFloat4(actual, expected.x, expected.y, expected.z, expected.w);
    }

    private static void AssertFloat4(Float4 actual, float x, float y, float z, float w)
    {
        Assert.That(actual.x, Is.EqualTo(x).Within(Epsilon));
        Assert.That(actual.y, Is.EqualTo(y).Within(Epsilon));
        Assert.That(actual.z, Is.EqualTo(z).Within(Epsilon));
        Assert.That(actual.w, Is.EqualTo(w).Within(Epsilon));
    }

    private static void AssertMat4IsFinite(Mat4 value)
    {
        AssertFloat4IsFinite(value.Column0);
        AssertFloat4IsFinite(value.Column1);
        AssertFloat4IsFinite(value.Column2);
        AssertFloat4IsFinite(value.Column3);
    }

    private static void AssertFloat4IsFinite(Float4 value)
    {
        Assert.That(float.IsFinite(value.x), Is.True);
        Assert.That(float.IsFinite(value.y), Is.True);
        Assert.That(float.IsFinite(value.z), Is.True);
        Assert.That(float.IsFinite(value.w), Is.True);
    }
}

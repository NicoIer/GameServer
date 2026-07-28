using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class BodyInterfaceTests : JoltTestBase
{
    [Test]
    public void CreateAndAddBody_ReturnsValidBodyID()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        Assert.That(id.IsValid, Is.True);
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void CreateBody_ThenAddBody()
    {
        using var shape = new SphereShape(0.5f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        var id = BodyInterface.CreateBody(settings);
        Assert.That(id.IsValid, Is.True);
        Assert.That(BodyInterface.IsAdded(id), Is.False);

        BodyInterface.AddBody(id, Activation.Activate);
        Assert.That(BodyInterface.IsAdded(id), Is.True);

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void SetPosition_GetPosition()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var target = new Float3(5f, 10f, 15f);
        BodyInterface.SetPosition(id, target, Activation.Activate);

        var pos = BodyInterface.GetPosition(id);
        Assert.That(pos.x, Is.EqualTo(5f).Within(1e-4f));
        Assert.That(pos.y, Is.EqualTo(10f).Within(1e-4f));
        Assert.That(pos.z, Is.EqualTo(15f).Within(1e-4f));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void SetRotation_GetRotation()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        // 90 degrees around Y axis
        float s = MathF.Sin(MathF.PI / 4f);
        float c = MathF.Cos(MathF.PI / 4f);
        var rot = new Quat(0f, s, 0f, c);
        BodyInterface.SetRotation(id, rot, Activation.Activate);

        var result = BodyInterface.GetRotation(id);
        Assert.That(result.y, Is.EqualTo(s).Within(1e-4f));
        Assert.That(result.w, Is.EqualTo(c).Within(1e-4f));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void LinearVelocity_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var vel = new Float3(1f, 2f, 3f);
        BodyInterface.SetLinearVelocity(id, vel);

        var result = BodyInterface.GetLinearVelocity(id);
        Assert.That(result.x, Is.EqualTo(1f).Within(1e-4f));
        Assert.That(result.y, Is.EqualTo(2f).Within(1e-4f));
        Assert.That(result.z, Is.EqualTo(3f).Within(1e-4f));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void AngularVelocity_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var vel = new Float3(0f, 5f, 0f);
        BodyInterface.SetAngularVelocity(id, vel);

        var result = BodyInterface.GetAngularVelocity(id);
        Assert.That(result.y, Is.EqualTo(5f).Within(1e-4f));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void ObjectLayer_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetObjectLayer(id, LayerStatic);

        var layer = BodyInterface.GetObjectLayer(id);
        Assert.That(layer, Is.EqualTo(LayerStatic));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void CollisionGroup_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var group = new CollisionGroup(42, 7);
        BodyInterface.SetCollisionGroup(id, group);

        var result = BodyInterface.GetCollisionGroup(id);
        Assert.That(result.groupID, Is.EqualTo(42u));
        Assert.That(result.subGroupID, Is.EqualTo(7u));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void Friction_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetFriction(id, 0.75f);
        Assert.That(BodyInterface.GetFriction(id), Is.EqualTo(0.75f).Within(1e-6f));
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void Restitution_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetRestitution(id, 0.9f);
        Assert.That(BodyInterface.GetRestitution(id), Is.EqualTo(0.9f).Within(1e-6f));
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void GravityFactor_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetGravityFactor(id, 0.0f);
        Assert.That(BodyInterface.GetGravityFactor(id), Is.EqualTo(0.0f).Within(1e-6f));
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void MotionType_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        Assert.That(BodyInterface.GetMotionType(id), Is.EqualTo(MotionType.Dynamic));

        BodyInterface.SetMotionType(id, MotionType.Kinematic, Activation.Activate);
        Assert.That(BodyInterface.GetMotionType(id), Is.EqualTo(MotionType.Kinematic));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void MotionQuality_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetMotionQuality(id, MotionQuality.LinearCast);
        Assert.That(BodyInterface.GetMotionQuality(id), Is.EqualTo(MotionQuality.LinearCast));
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void IsSensor_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetIsSensor(id, true);
        Assert.That(BodyInterface.IsSensor(id), Is.True);
        BodyInterface.SetIsSensor(id, false);
        Assert.That(BodyInterface.IsSensor(id), Is.False);
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void ActivateAndDeactivate()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        Assert.That(BodyInterface.IsActive(id), Is.True);

        BodyInterface.DeactivateBody(id);
        Assert.That(BodyInterface.IsActive(id), Is.False);

        BodyInterface.ActivateBody(id);
        Assert.That(BodyInterface.IsActive(id), Is.True);

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void UserData_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetUserData(id, 12345678UL);
        Assert.That(BodyInterface.GetUserData(id), Is.EqualTo(12345678UL));
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void AddForce_DoesNotThrow()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        Assert.DoesNotThrow(() => BodyInterface.AddForce(id, new Float3(0, 100, 0)));
        Assert.DoesNotThrow(() => BodyInterface.AddForce(id, new Float3(0, 100, 0), new Float3(0.1f, 0, 0)));
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void AddImpulse_ChangesVelocity()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        BodyInterface.SetLinearVelocity(id, Float3.Zero);
        BodyInterface.AddImpulse(id, new Float3(10f, 0, 0));

        var vel = BodyInterface.GetLinearVelocity(id);
        Assert.That(vel.x, Is.GreaterThan(0f));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void CenterOfMassPosition_IsValid()
    {
        var id = CreateDynamicSphere(new Float3(5f, 5f, 5f));
        var com = BodyInterface.GetCenterOfMassPosition(id);
        Assert.That(com.x, Is.EqualTo(5f).Within(1e-4f));
        Assert.That(com.y, Is.EqualTo(5f).Within(1e-4f));
        Assert.That(com.z, Is.EqualTo(5f).Within(1e-4f));
        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void CreateBody_ReturnsInvalid_WhenMaxBodiesExceeded()
    {
        const uint maxBodies = 4;

        // Create a dedicated PhysicsSystem with very small maxBodies
        using var bpli = new BroadPhaseLayerInterfaceTable(NumObjectLayers, NumBroadPhaseLayers);
        bpli.MapObjectToBroadPhaseLayer(LayerStatic, BPLayerStatic);
        bpli.MapObjectToBroadPhaseLayer(LayerDynamic, BPLayerDynamic);

        using var olpf = new ObjectLayerPairFilterTable(NumObjectLayers);
        olpf.EnableCollision(LayerStatic, LayerDynamic);
        olpf.EnableCollision(LayerDynamic, LayerDynamic);

        using var obvbpf = new ObjectVsBroadPhaseLayerFilterTable(
            bpli, NumBroadPhaseLayers, olpf, NumObjectLayers);

        var settings = new PhysicsSystemSettings
        {
            MaxBodies = maxBodies,
            NumBodyMutexes = 0,
            MaxBodyPairs = 1024,
            MaxContactConstraints = 1024,
        };

        using var ps = new PhysicsSystem(settings, bpli, olpf, obvbpf);
        var bi = ps.GetBodyInterface();

        // Fill up all body slots
        var ids = new List<BodyID>();
        using var shape = new SphereShape(0.5f);

        for (uint i = 0; i < maxBodies; i++)
        {
            using var bodySettings = new BodyCreationSettings(
                shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
            var id = bi.CreateBody(bodySettings);
            Assert.That(id.IsValid, Is.True, $"Body {i} should be valid");
            ids.Add(id);
        }

        // Next create should fail — maxBodies exceeded
        using var extraSettings = new BodyCreationSettings(
            shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        var extraId = bi.CreateBody(extraSettings);
        Assert.That(extraId.IsValid, Is.False, "Body beyond maxBodies should be invalid");

        // Cleanup
        foreach (var id in ids)
            bi.DestroyBody(id);
    }
}

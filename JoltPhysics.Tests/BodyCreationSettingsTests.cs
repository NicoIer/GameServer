using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class BodyCreationSettingsTests : JoltTestBase
{
    [Test]
    public void Friction_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.Friction = 0.42f;
        Assert.That(settings.Friction, Is.EqualTo(0.42f).Within(1e-6f));
    }

    [Test]
    public void Restitution_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.Restitution = 0.8f;
        Assert.That(settings.Restitution, Is.EqualTo(0.8f).Within(1e-6f));
    }

    [Test]
    public void AllowSleeping_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.AllowSleeping = false;
        Assert.That(settings.AllowSleeping, Is.False);
        settings.AllowSleeping = true;
        Assert.That(settings.AllowSleeping, Is.True);
    }

    [Test]
    public void MotionQuality_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.MotionQuality = MotionQuality.LinearCast;
        Assert.That(settings.MotionQuality, Is.EqualTo(MotionQuality.LinearCast));
    }

    [Test]
    public void GravityFactor_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.GravityFactor = 0.5f;
        Assert.That(settings.GravityFactor, Is.EqualTo(0.5f).Within(1e-6f));
    }

    [Test]
    public void IsSensor_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.IsSensor = true;
        Assert.That(settings.IsSensor, Is.True);
        settings.IsSensor = false;
        Assert.That(settings.IsSensor, Is.False);
    }

    [Test]
    public void LinearDamping_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.LinearDamping = 0.3f;
        Assert.That(settings.LinearDamping, Is.EqualTo(0.3f).Within(1e-6f));
    }

    [Test]
    public void AngularDamping_SetAndGet()
    {
        using var shape = new SphereShape(1f);
        using var settings = new BodyCreationSettings(shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.AngularDamping = 0.1f;
        Assert.That(settings.AngularDamping, Is.EqualTo(0.1f).Within(1e-6f));
    }
}

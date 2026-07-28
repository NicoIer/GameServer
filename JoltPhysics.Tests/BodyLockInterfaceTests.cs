using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class BodyLockInterfaceTests : JoltTestBase
{
    [Test]
    public void GetBroadPhaseLayer_ReturnsExpected()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();

        var bpLayer = lockInterface.GetBroadPhaseLayer(id);
        Assert.That(bpLayer, Is.EqualTo(BPLayerDynamic));

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void AllowSleeping_SetAndGet()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();

        lockInterface.SetAllowSleeping(id, false);
        Assert.That(lockInterface.GetAllowSleeping(id), Is.False);

        lockInterface.SetAllowSleeping(id, true);
        Assert.That(lockInterface.GetAllowSleeping(id), Is.True);

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void LockRead_Succeeded_ForValidBody()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();

        using var lockRead = lockInterface.LockRead(id);
        Assert.That(lockRead.Succeeded, Is.True);

        BodyInterface.RemoveAndDestroyBody(id);
    }

    [Test]
    public void LockRead_NotSucceeded_ForInvalidBody()
    {
        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();

        using var lockRead = lockInterface.LockRead(BodyID.Invalid);
        Assert.That(lockRead.Succeeded, Is.False);
    }

    [Test]
    public void LockWrite_Succeeded_ForValidBody()
    {
        var id = CreateDynamicSphere(Float3.Zero);
        var lockInterface = PhysicsSystem.GetBodyLockInterfaceNoLock();

        using var lockWrite = lockInterface.LockWrite(id);
        Assert.That(lockWrite.Succeeded, Is.True);

        BodyInterface.RemoveAndDestroyBody(id);
    }
}

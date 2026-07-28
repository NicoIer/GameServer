using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class ContactListenerTests : JoltTestBase
{
    [Test]
    public void ContactListener_CanCreateAndDispose()
    {
        using var listener = new ContactListener();
        Assert.That(listener.IsDisposed, Is.False);
    }

    [Test]
    public void ContactListener_SetOnPhysicsSystem()
    {
        using var listener = new ContactListener();
        listener.OnContactAdded = (_, _) => { };
        PhysicsSystem.SetContactListener(listener);
        Assert.Pass("ContactListener set without error");
    }

    [Test]
    public void ContactListener_DisposeAfterSet()
    {
        var listener = new ContactListener();
        PhysicsSystem.SetContactListener(listener);
        listener.Dispose();
        Assert.That(listener.IsDisposed, Is.True);
    }

    [Test]
    public void ContactAdded_Fires()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        listener.OnContactAdded = (_, _) => { addedCount++; };

        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = new Float3(0, -10f, 0);

        var floor = CreateStaticBox(new Float3(0, -5f, 0), new Float3(50f, 5f, 50f));
        var sphere = CreateDynamicSphere(new Float3(0, 0.5f, 0), 0.5f);

        for (int i = 0; i < 30; i++)
            Step();

        Assert.That(addedCount, Is.GreaterThan(0), "OnContactAdded should have fired");

        BodyInterface.RemoveAndDestroyBody(sphere);
        BodyInterface.RemoveAndDestroyBody(floor);
    }

    [Test]
    public void ContactRemoved_FiresAfterRemoveBody()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        int removedCount = 0;

        listener.OnContactAdded = (_, _) => { addedCount++; };
        listener.OnContactRemoved = (_, _) => { removedCount++; };

        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = new Float3(0, -10f, 0);

        var floor = CreateStaticBox(new Float3(0, -5f, 0), new Float3(50f, 5f, 50f));
        var sphere = CreateDynamicSphere(new Float3(0, 0.5f, 0), 0.5f);

        // Let contact happen
        for (int i = 0; i < 30; i++)
            Step();

        Assert.That(addedCount, Is.GreaterThan(0), "OnContactAdded should have fired");

        // Remove body outside of callback
        BodyInterface.RemoveBody(sphere);

        // Step to propagate removal
        for (int i = 0; i < 5; i++)
            Step();

        Assert.That(removedCount, Is.GreaterThan(0), "OnContactRemoved should fire after RemoveBody");
        
        TestContext.WriteLine($"Added: {addedCount}, Removed: {removedCount}");

        BodyInterface.DestroyBody(sphere);
        BodyInterface.RemoveAndDestroyBody(floor);
    }

    /// <summary>
    /// 在 OnContactAdded 中标记需要移除，Step 结束后 RemoveBody，验证 OnContactRemoved 是否触发。
    /// 注意：不能在回调中直接调用 BodyInterface（会死锁），必须在 Update 外操作。
    /// </summary>
    [Test]
    public void ContactRemoved_FiresWhenRemoveAfterContactAdded()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        int removedCount = 0;
        bool shouldRemove = false;

        listener.OnContactAdded = (_, _) =>
        {
            addedCount++;
            shouldRemove = true; // 仅标记，不在回调中操作 BodyInterface
        };
        listener.OnContactRemoved = (_, _) => { removedCount++; };

        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = new Float3(0, -10f, 0);

        var floor = CreateStaticBox(new Float3(0, -5f, 0), new Float3(50f, 5f, 50f));
        var sphere = CreateDynamicSphere(new Float3(0, 0.5f, 0), 0.5f);

        // Step 直到 contact 触发
        for (int i = 0; i < 30 && !shouldRemove; i++)
            Step();

        Assert.That(addedCount, Is.GreaterThan(0), "OnContactAdded should have fired");

        // 在 Update 外移除
        BodyInterface.RemoveBody(sphere);

        // 再 Step 让 Jolt 检测到 contact 丢失
        for (int i = 0; i < 5; i++)
            Step();

        TestContext.WriteLine($"Added: {addedCount}, Removed: {removedCount}");
        Assert.That(removedCount, Is.GreaterThan(0), "OnContactRemoved should fire after RemoveBody");

        BodyInterface.DestroyBody(sphere);
        BodyInterface.RemoveAndDestroyBody(floor);
    }

    /// <summary>
    /// 只创建并 Add 重叠的 body，不调用 Step()，验证不会触发任何 Contact 事件。
    /// Contact 事件仅在 PhysicsSystem.Update (Step) 中才会检测和派发。
    /// </summary>
    [Test]
    public void NoContactEvents_WithoutStep()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        int persistedCount = 0;
        int removedCount = 0;

        listener.OnContactAdded = (_, _) => { addedCount++; };
        listener.OnContactPersisted = (_, _) => { persistedCount++; };
        listener.OnContactRemoved = (_, _) => { removedCount++; };

        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = new Float3(0, -10f, 0);

        // 创建重叠的 body：球心在地板表面内部，确保有碰撞
        var floor = CreateStaticBox(new Float3(0, 0, 0), new Float3(50f, 5f, 50f));
        var sphere = CreateDynamicSphere(new Float3(0, 0, 0), 0.5f);

        // 不调用 Step()

        Assert.That(addedCount, Is.EqualTo(0), "OnContactAdded should NOT fire without Step");
        Assert.That(persistedCount, Is.EqualTo(0), "OnContactPersisted should NOT fire without Step");
        Assert.That(removedCount, Is.EqualTo(0), "OnContactRemoved should NOT fire without Step");

        BodyInterface.RemoveAndDestroyBody(sphere);
        BodyInterface.RemoveAndDestroyBody(floor);
    }
    
    [Test]
    public void ContactEvents_WithStep()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        int persistedCount = 0;
        int removedCount = 0;

        listener.OnContactAdded = (_, _) => { addedCount++; };
        listener.OnContactPersisted = (_, _) => { persistedCount++; };
        listener.OnContactRemoved = (_, _) => { removedCount++; };

        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = new Float3(0, -10f, 0);

        // 创建重叠的 body：球心在地板表面内部，确保有碰撞
        var floor = CreateStaticBox(new Float3(0, 0, 0), new Float3(50f, 5f, 50f));
        var sphere = CreateDynamicSphere(new Float3(0, 0, 0), 0.5f);

        // 不调用 Step()

        Assert.That(addedCount, Is.EqualTo(0), "OnContactAdded should NOT fire without Step");
        Assert.That(persistedCount, Is.EqualTo(0), "OnContactPersisted should NOT fire without Step");
        Assert.That(removedCount, Is.EqualTo(0), "OnContactRemoved should NOT fire without Step");
        
        for (int i = 0; i < 5; i++)
            Step();
        
        
        Assert.That(addedCount, Is.GreaterThan(0), "OnContactAdded should fire after Step");
        Assert.That(persistedCount, Is.GreaterThan(0), "OnContactPersisted should fire after Step");
        // Assert.That(removedCount, Is.EqualTo(0), "OnContactRemoved should NOT fire yet");
        

        BodyInterface.RemoveAndDestroyBody(sphere);
        BodyInterface.RemoveAndDestroyBody(floor);
    }
    

    [Test]
    public void ContactPersisted_FiresOnSustainedContact()
    {
        using var listener = new ContactListener();
        int persistedCount = 0;
        listener.OnContactAdded = (_, _) => { };
        listener.OnContactPersisted = (_, _) => { persistedCount++; };

        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = new Float3(0, -10f, 0);

        var floor = CreateStaticBox(new Float3(0, -5f, 0), new Float3(50f, 5f, 50f));
        var sphere = CreateDynamicSphere(new Float3(0, 0.5f, 0), 0.5f);

        for (int i = 0; i < 30; i++)
            Step();

        Assert.That(persistedCount, Is.GreaterThan(0), "OnContactPersisted should fire on sustained contact");

        BodyInterface.RemoveAndDestroyBody(sphere);
        BodyInterface.RemoveAndDestroyBody(floor);
    }
}

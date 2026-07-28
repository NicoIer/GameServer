using JoltPhysics;

namespace JoltPhysics.Test;

[TestFixture]
public class CollisionGroupTests : JoltTestBase
{
    /// <summary>
    /// GroupFilterTable 基本创建和销毁。
    /// </summary>
    [Test]
    public void GroupFilterTable_CreateAndDispose()
    {
        using var table = new GroupFilterTable(4);
        Assert.That(table.IsDisposed, Is.False);
    }

    /// <summary>
    /// 验证 EnableCollision / DisableCollision / IsCollisionEnabled 基本逻辑。
    /// </summary>
    [Test]
    public void GroupFilterTable_EnableDisableQuery()
    {
        using var table = new GroupFilterTable(4);

        // Jolt 默认全部启用
        Assert.That(table.IsCollisionEnabled(0, 1), Is.True);

        table.DisableCollision(0, 1);
        Assert.That(table.IsCollisionEnabled(0, 1), Is.False);

        table.EnableCollision(0, 1);
        Assert.That(table.IsCollisionEnabled(0, 1), Is.True);
    }

    /// <summary>
    /// 验证 CollisionGroup 构造函数正确保留字段值。
    /// </summary>
    [Test]
    public void CollisionGroup_Constructor_PreservesFields()
    {
        var group = new CollisionGroup(42, 7);
        Assert.That(group.groupID, Is.EqualTo(42));
        Assert.That(group.subGroupID, Is.EqualTo(7));
        Assert.That(group.groupFilter, Is.Null);

        using var table = new GroupFilterTable(4);
        var groupWithFilter = new CollisionGroup(table, 10, 3);
        Assert.That(groupWithFilter.groupFilter, Is.SameAs(table));
        Assert.That(groupWithFilter.groupID, Is.EqualTo(10));
        Assert.That(groupWithFilter.subGroupID, Is.EqualTo(3));
    }

    /// <summary>
    /// 通过 BodyInterface 设置 CollisionGroup（含 GroupFilter），验证 groupID/subGroupID 正确回读。
    /// </summary>
    [Test]
    public void BodyInterface_SetGetCollisionGroup_WithGroupFilter()
    {
        using var table = new GroupFilterTable(4);
        table.EnableCollision(0, 1);

        var bodyId = CreateDynamicSphere(new Float3(0, 5, 0));

        var group = new CollisionGroup(table, 10, 2);
        BodyInterface.SetCollisionGroup(bodyId, group);

        var readBack = BodyInterface.GetCollisionGroup(bodyId);
        Assert.That(readBack.groupID, Is.EqualTo(10));
        Assert.That(readBack.subGroupID, Is.EqualTo(2));

        BodyInterface.RemoveAndDestroyBody(bodyId);
    }

    /// <summary>
    /// 通过 BodyCreationSettings 设置 CollisionGroup（含 GroupFilter），验证写入生效。
    /// </summary>
    [Test]
    public void BodyCreationSettings_CollisionGroup_RoundTrip()
    {
        using var table = new GroupFilterTable(8);
        table.EnableCollision(1, 2);

        using var shape = new SphereShape(0.5f);
        using var settings = new BodyCreationSettings(
            shape, Float3.Zero, Quat.Identity, MotionType.Dynamic, LayerDynamic);

        var group = new CollisionGroup(table, 5, 3);
        settings.CollisionGroup = group;

        var readBack = settings.CollisionGroup;
        Assert.That(readBack.groupID, Is.EqualTo(5));
        Assert.That(readBack.subGroupID, Is.EqualTo(3));
    }

    /// <summary>
    /// 核心测试：同 GroupID、不同 SubGroup，禁用碰撞后不产生 contact；启用后产生 contact。
    /// 两个球重叠放置，通过 GroupFilterTable 控制是否碰撞。
    /// </summary>
    [Test]
    public void GroupFilter_DisabledSubGroups_NoContact()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        listener.OnContactAdded = (_, _) => { addedCount++; };
        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = Float3.Zero;

        using var table = new GroupFilterTable(4);
        // SubGroup 0 和 1 之间禁止碰撞（默认即禁止，此处显式确认）
        table.DisableCollision(0, 1);

        // 创建两个重叠球体，同 GroupID=1，不同 SubGroup (0 和 1)
        var bodyA = CreateDynamicSphereWithGroup(new Float3(0, 0, 0), table, groupID: 1, subGroupID: 0);
        var bodyB = CreateDynamicSphereWithGroup(new Float3(0, 0.1f, 0), table, groupID: 1, subGroupID: 1);

        for (int i = 0; i < 10; i++)
            Step();

        Assert.That(addedCount, Is.EqualTo(0),
            "Bodies with disabled sub-group collision should NOT generate contacts");

        BodyInterface.RemoveAndDestroyBody(bodyA);
        BodyInterface.RemoveAndDestroyBody(bodyB);
    }

    [Test]
    public void GroupFilter_EnabledSubGroups_HasContact()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        listener.OnContactAdded = (_, _) => { addedCount++; };
        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = Float3.Zero;

        using var table = new GroupFilterTable(4);
        table.EnableCollision(0, 1);

        // 创建两个重叠球体，同 GroupID=1，不同 SubGroup (0 和 1)
        var bodyA = CreateDynamicSphereWithGroup(new Float3(0, 0, 0), table, groupID: 1, subGroupID: 0);
        var bodyB = CreateDynamicSphereWithGroup(new Float3(0, 0.1f, 0), table, groupID: 1, subGroupID: 1);

        for (int i = 0; i < 10; i++)
            Step();

        Assert.That(addedCount, Is.GreaterThan(0),
            "Bodies with enabled sub-group collision should generate contacts");

        BodyInterface.RemoveAndDestroyBody(bodyA);
        BodyInterface.RemoveAndDestroyBody(bodyB);
    }

    /// <summary>
    /// 不同 GroupID 的 body 不受 GroupFilterTable 影响，始终碰撞（由 ObjectLayerPairFilter 决定）。
    /// </summary>
    [Test]
    public void GroupFilter_DifferentGroupIDs_AlwaysCollide()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        listener.OnContactAdded = (_, _) => { addedCount++; };
        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = Float3.Zero;

        using var table = new GroupFilterTable(4);
        // SubGroup 0 和 1 之间禁止碰撞
        table.DisableCollision(0, 1);

        // 不同 GroupID → GroupFilter 不生效，仍然碰撞
        var bodyA = CreateDynamicSphereWithGroup(new Float3(0, 0, 0), table, groupID: 1, subGroupID: 0);
        var bodyB = CreateDynamicSphereWithGroup(new Float3(0, 0.1f, 0), table, groupID: 2, subGroupID: 1);

        for (int i = 0; i < 10; i++)
            Step();

        Assert.That(addedCount, Is.GreaterThan(0),
            "Bodies with different GroupIDs should always collide (GroupFilter only filters same GroupID)");

        BodyInterface.RemoveAndDestroyBody(bodyA);
        BodyInterface.RemoveAndDestroyBody(bodyB);
    }

    /// <summary>
    /// 无 GroupFilter 时，同 GroupID 不同 SubGroup 的 body 默认碰撞。
    /// </summary>
    [Test]
    public void NoGroupFilter_DefaultCollision()
    {
        using var listener = new ContactListener();
        int addedCount = 0;
        listener.OnContactAdded = (_, _) => { addedCount++; };
        PhysicsSystem.SetContactListener(listener);
        PhysicsSystem.Gravity = Float3.Zero;

        // 不设 GroupFilter
        var bodyA = CreateDynamicSphereWithGroup(new Float3(0, 0, 0), null, groupID: 1, subGroupID: 0);
        var bodyB = CreateDynamicSphereWithGroup(new Float3(0, 0.1f, 0), null, groupID: 1, subGroupID: 1);

        for (int i = 0; i < 10; i++)
            Step();

        Assert.That(addedCount, Is.GreaterThan(0),
            "Without GroupFilter, bodies should collide by default");

        BodyInterface.RemoveAndDestroyBody(bodyA);
        BodyInterface.RemoveAndDestroyBody(bodyB);
    }

    /// <summary>
    /// Helper: 创建带 CollisionGroup 的 dynamic sphere。
    /// </summary>
    private BodyID CreateDynamicSphereWithGroup(Float3 position, GroupFilter? filter,
        uint groupID, uint subGroupID, float radius = 0.5f)
    {
        using var shape = new SphereShape(radius);
        using var settings = new BodyCreationSettings(
            shape, position, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        settings.CollisionGroup = filter != null
            ? new CollisionGroup(filter, groupID, subGroupID)
            : new CollisionGroup(groupID, subGroupID);
        return BodyInterface.CreateAndAddBody(settings, Activation.Activate);
    }
}


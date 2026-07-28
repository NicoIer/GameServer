using JoltPhysics;

namespace JoltPhysics.Test;

/// <summary>
/// Base class for Jolt tests. Handles JoltAPI init/shutdown and creates a standard PhysicsSystem.
/// </summary>
public class JoltTestBase
{
    protected PhysicsSystem PhysicsSystem = null!;
    protected BodyInterface BodyInterface;
    protected BroadPhaseLayerInterfaceTable BroadPhaseLayerInterface = null!;
    protected ObjectLayerPairFilterTable ObjectLayerPairFilter = null!;
    protected ObjectVsBroadPhaseLayerFilterTable ObjectVsBroadPhaseLayerFilter = null!;
    protected TempAllocator TempAllocator = null!;
    protected JobSystemThreadPool JobSystem = null!;

    // Standard layers
    protected static readonly ObjectLayer LayerStatic = 0;
    protected static readonly ObjectLayer LayerDynamic = 1;
    protected const uint NumObjectLayers = 2;
    protected static readonly BroadPhaseLayer BPLayerStatic = 0;
    protected static readonly BroadPhaseLayer BPLayerDynamic = 1;
    protected const uint NumBroadPhaseLayers = 2;

    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        Assert.That(JoltAPI.Init(), Is.True, "JoltAPI.Init() failed");
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        JoltAPI.Shutdown();
    }

    [SetUp]
    public void SetUp()
    {
        BroadPhaseLayerInterface = new BroadPhaseLayerInterfaceTable(NumObjectLayers, NumBroadPhaseLayers);
        BroadPhaseLayerInterface.MapObjectToBroadPhaseLayer(LayerStatic, BPLayerStatic);
        BroadPhaseLayerInterface.MapObjectToBroadPhaseLayer(LayerDynamic, BPLayerDynamic);

        ObjectLayerPairFilter = new ObjectLayerPairFilterTable(NumObjectLayers);
        ObjectLayerPairFilter.EnableCollision(LayerStatic, LayerDynamic);
        ObjectLayerPairFilter.EnableCollision(LayerDynamic, LayerDynamic);

        ObjectVsBroadPhaseLayerFilter = new ObjectVsBroadPhaseLayerFilterTable(
            BroadPhaseLayerInterface, NumBroadPhaseLayers,
            ObjectLayerPairFilter, NumObjectLayers);

        PhysicsSystem = new PhysicsSystem(
            PhysicsSystemSettings.Default,
            BroadPhaseLayerInterface,
            ObjectLayerPairFilter,
            ObjectVsBroadPhaseLayerFilter);

        BodyInterface = PhysicsSystem.GetBodyInterface();

        TempAllocator = new TempAllocator(16 * 1024 * 1024);
        JobSystem = new JobSystemThreadPool();
    }

    [TearDown]
    public void TearDown()
    {
        PhysicsSystem?.Dispose();
        TempAllocator?.Dispose();
        JobSystem?.Dispose();
        ObjectVsBroadPhaseLayerFilter?.Dispose();
        ObjectLayerPairFilter?.Dispose();
        BroadPhaseLayerInterface?.Dispose();
    }

    /// <summary>
    /// Helper: create a dynamic sphere body at the given position, added to the world.
    /// </summary>
    protected BodyID CreateDynamicSphere(Float3 position, float radius = 0.5f)
    {
        using var shape = new SphereShape(radius);
        using var settings = new BodyCreationSettings(shape, position, Quat.Identity, MotionType.Dynamic, LayerDynamic);
        return BodyInterface.CreateAndAddBody(settings, Activation.Activate);
    }

    /// <summary>
    /// Helper: create a static box body at the given position, added to the world.
    /// </summary>
    protected BodyID CreateStaticBox(Float3 position, Float3 halfExtents)
    {
        using var shape = new BoxShape(halfExtents);
        using var settings = new BodyCreationSettings(shape, position, Quat.Identity, MotionType.Static, LayerStatic);
        return BodyInterface.CreateAndAddBody(settings, Activation.DontActivate);
    }

    /// <summary>
    /// Step the physics world.
    /// </summary>
    protected void Step(float dt = 1f / 60f, int collisionSteps = 1)
    {
        PhysicsSystem.Update(dt, collisionSteps, TempAllocator, JobSystem);
    }

    /// <summary>
    /// Wait until the body is added, then step once so broadphase/narrowphase queries can see it.
    /// </summary>
    protected void WaitForBodyAddedAndReadyForQuery(BodyID bodyID, int maxSteps = 3)
    {
        for (int i = 0; i < maxSteps && !BodyInterface.IsAdded(bodyID); i++)
            Step();

        Assert.That(BodyInterface.IsAdded(bodyID), Is.True, $"Body {bodyID} was not added to the physics world.");
        Step();
    }
}

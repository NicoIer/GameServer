using System.Diagnostics;
using System.Numerics;
using JoltPhysicsSharp;
using Raylib_cs;

namespace JoltServer;

public class JoltApplication : DisposableObject
{
    private PhysicsSystemSettings _settings;

    public IReadOnlyList<BodyID> bodies => _bodies;
    public IReadOnlySet<BodyID> ignoreDrawBodies => _ignoreDrawBodies;

    protected readonly List<BodyID> _bodies = [];
    protected readonly HashSet<BodyID> _ignoreDrawBodies = [];
    public int targetFPS => TargetFPS;
    public JobSystem jobSystem { get; set; }
    public PhysicsSystem physicsSystem { get; private set; }


    protected const int TargetFPS = 60;

    private const int MaxBodies = 65536;
    private const int MaxBodyPairs = 65536;
    private const int MaxContactConstraints = 65536;
    private const int NumBodyMutexes = 0;

    internal static class Layers
    {
        public static readonly ObjectLayer NonMoving = 0;
        public static readonly ObjectLayer Moving = 1;
    }

    internal static class BroadPhaseLayers
    {
        public static readonly BroadPhaseLayer NonMoving = 0;
        public static readonly BroadPhaseLayer Moving = 1;
    }


    public JoltApplication()
    {
        if (!Foundation.Init(false)) return;
        systems = new List<ISystem>();
        Foundation.SetTraceHandler((message => Console.WriteLine(message)));
#if DEBUG
        Foundation.SetAssertFailureHandler((inExpression, inMessage, inFile, inLine) =>
        {
            string message = inMessage ?? inExpression;

            string outMessage = $"[JoltPhysics] Assertion failure at {inFile}:{inLine}: {message}";

            Debug.WriteLine(outMessage);

            throw new Exception(outMessage);
        });
#endif
        _settings = new PhysicsSystemSettings()
        {
            MaxBodies = MaxBodies,
            MaxBodyPairs = MaxBodyPairs,
            MaxContactConstraints = MaxContactConstraints,
            NumBodyMutexes = NumBodyMutexes,
        };

        jobSystem = new JobSystemThreadPool();
        SetupCollisionFiltering();
        physicsSystem = new PhysicsSystem(_settings);

        // ContactListener
        physicsSystem.OnContactValidate += OnContactValidate;
        physicsSystem.OnContactAdded += OnContactAdded;
        physicsSystem.OnContactPersisted += OnContactPersisted;
        physicsSystem.OnContactRemoved += OnContactRemoved;
        // BodyActivationListener
        physicsSystem.OnBodyActivated += OnBodyActivated;
        physicsSystem.OnBodyDeactivated += OnBodyDeactivated;
    }

    #region Physics

    protected virtual void SetupCollisionFiltering()
    {
        // We use only 2 layers: one for non-moving objects and one for moving objects
        ObjectLayerPairFilterTable objectLayerPairFilter = new(2);
        objectLayerPairFilter.EnableCollision(Layers.NonMoving, Layers.Moving);
        objectLayerPairFilter.EnableCollision(Layers.Moving, Layers.Moving);

        // We use a 1-to-1 mapping between object layers and broadphase layers
        BroadPhaseLayerInterfaceTable broadPhaseLayerInterface = new(2, 2);
        broadPhaseLayerInterface.MapObjectToBroadPhaseLayer(Layers.NonMoving, BroadPhaseLayers.NonMoving);
        broadPhaseLayerInterface.MapObjectToBroadPhaseLayer(Layers.Moving, BroadPhaseLayers.Moving);

        ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter =
            new(broadPhaseLayerInterface, 2, objectLayerPairFilter, 2);

        _settings.ObjectLayerPairFilter = objectLayerPairFilter;
        _settings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
        _settings.ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;
    }

    internal BodyID CreateFloor(float size, ObjectLayer layer)
    {
        BoxShape shape = new(new Vector3(size, 5.0f, size));
        using BodyCreationSettings creationSettings =
            new(shape, new Vector3(0, -5.0f, 0.0f), Quaternion.Identity, MotionType.Static, layer);
        BodyID body = physicsSystem.BodyInterface.CreateAndAddBody(creationSettings, Activation.DontActivate);
        _bodies.Add(body);
        // _ignoreDrawBodies.Add(body);
        return body;
    }

    internal BodyID CreateBox(in Vector3 halfExtent,
        in Vector3 position,
        in Quaternion rotation,
        MotionType motionType,
        ObjectLayer layer,
        Activation activation = Activation.Activate)
    {
        BoxShape shape = new(halfExtent);
        using BodyCreationSettings creationSettings = new(shape, position, rotation, motionType, layer);
        BodyID body = physicsSystem.BodyInterface.CreateAndAddBody(creationSettings, activation);
        _bodies.Add(body);
        return body;
    }

    protected BodyID CreateSphere(float radius,
        in Vector3 position,
        in Quaternion rotation,
        MotionType motionType,
        ObjectLayer layer,
        Activation activation = Activation.Activate)
    {
        SphereShape shape = new(radius);
        using BodyCreationSettings creationSettings = new(shape, position, rotation, motionType, layer);
        BodyID body = physicsSystem.BodyInterface.CreateAndAddBody(creationSettings, activation);
        _bodies.Add(body);
        return body;
    }

    protected virtual ValidateResult OnContactValidate(PhysicsSystem system, in Body body1, in Body body2,
        Double3 baseOffset, in CollideShapeResult collisionResult)
    {
        // TraceLog(TraceLogLevel.Debug, "Contact validate callback");

        // Allows you to ignore a contact before it is created (using layers to not make objects collide is cheaper!)
        return ValidateResult.AcceptAllContactsForThisBodyPair;
    }

    protected virtual void OnContactAdded(PhysicsSystem system, in Body body1, in Body body2,
        in ContactManifold manifold, in ContactSettings settings)
    {
        // TraceLog(TraceLogLevel.Debug, "A contact was added");
    }

    protected virtual void OnContactPersisted(PhysicsSystem system, in Body body1, in Body body2,
        in ContactManifold manifold, in ContactSettings settings)
    {
        // TraceLog(TraceLogLevel.Debug, "A contact was persisted");
    }

    protected virtual void OnContactRemoved(PhysicsSystem system, ref SubShapeIDPair subShapePair)
    {
        // TraceLog(TraceLogLevel.Debug, "A contact was removed");
    }

    protected virtual void OnBodyActivated(PhysicsSystem system, in BodyID bodyID, ulong bodyUserData)
    {
        // TraceLog(TraceLogLevel.Debug, "A body got activated");
    }

    protected virtual void OnBodyDeactivated(PhysicsSystem system, in BodyID bodyID, ulong bodyUserData)
    {
        // TraceLog(TraceLogLevel.Debug, "A body went to sleep");
    }

    #endregion

    #region Systems

    public List<ISystem> systems { get; private set; }

    public delegate void UpdateAction(in LoopContex ctx);

    public interface ISystem : IDisposable
    {
        public JoltApplication application { get; }
        public void OnAdded(JoltApplication app);
        void OnRemoved();
        public void BeforeRun();
        public void BeforePhysicsUpdate(in LoopContex ctx);
        public void AfterPhysicsUpdate(in LoopContex ctx);
        public void AfterRun();

        public bool NeedShutdown()
        {
            return false;
        }
    }


    public event UpdateAction BeforePhysicsUpdate = delegate { };
    public event UpdateAction AfterPhysicsUpdate = delegate { };


    public void AddSystem(ISystem system)
    {
        systems.Add(system);
        system.OnAdded(this);
    }

    public void RemoveSystem(ISystem system)
    {
        systems.Remove(system);
        system.OnRemoved();
    }

    public bool GetSystem<T>(out T system) where T : ISystem
    {
        foreach (var s in systems)
        {
            if (s is T t)
            {
                system = t;
                return true;
            }
        }

        system = default!;
        return false;
    }

    #endregion

    public bool running { get; private set; }

    public struct LoopContex
    {
        public long CurrentFrame;
        public TimeSpan FrameBeginTimestamp;
        public long timeBetweenFrames;
        public TimeSpan ElapsedTimeFromPreviousFrame;
    }

    public void Run()
    {
        physicsSystem.OptimizeBroadPhase();
        const int collisionSteps = 1;
        float deltaTime = 1.0f / TargetFPS;
        // using var looper = new LogicLooper(TargetFPS);

        foreach (var system in systems)
        {
            system.BeforeRun();
        }

        running = true;
        Stopwatch stopwatch = new Stopwatch();
        LoopContex ctx = new LoopContex();
        stopwatch.Start();
        while (true)
        {
            ctx.CurrentFrame++;
            ctx.FrameBeginTimestamp = stopwatch.Elapsed;
            foreach (var system in systems)
            {
                system.BeforePhysicsUpdate(ctx);
            }

            BeforePhysicsUpdate(ctx);
            PhysicsUpdateError error = physicsSystem.Update(deltaTime, collisionSteps, jobSystem);
            Debug.Assert(error == PhysicsUpdateError.None);
            AfterPhysicsUpdate(ctx);

            foreach (var system in systems)
            {
                system.AfterPhysicsUpdate(ctx);
            }

            bool needShutdown = systems.Any(s => s.NeedShutdown());

            ctx.ElapsedTimeFromPreviousFrame = stopwatch.Elapsed - ctx.FrameBeginTimestamp;

            TimeSpan sleepTime = TimeSpan.FromMilliseconds(1000 / TargetFPS) - ctx.ElapsedTimeFromPreviousFrame;
            if (sleepTime > TimeSpan.Zero)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1000 / TargetFPS) - ctx.ElapsedTimeFromPreviousFrame);
            }
            if (needShutdown) break;
        }

        stopwatch.Stop();
        running = false;

        foreach (var system in systems)
        {
            system.AfterRun();
        }
    }


    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        foreach (BodyID bodyID in _bodies)
        {
            physicsSystem.BodyInterface.RemoveAndDestroyBody(bodyID);
        }

        _bodies.Clear();
        jobSystem.Dispose();
        physicsSystem.Dispose();
        foreach (var system in systems)
        {
            system.Dispose();
        }

        systems.Clear();
        Foundation.Shutdown();
    }
}
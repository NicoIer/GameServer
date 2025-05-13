using System.Diagnostics;
using System.Numerics;
using GameCore.Jolt;
using JoltPhysicsSharp;
using Raylib_cs;
using Serilog;
using UnityToolkit;
using Activation = JoltPhysicsSharp.Activation;
using MotionType = JoltPhysicsSharp.MotionType;

namespace JoltServer;

public class JoltApplication : DisposableObject
{
    private static long _worldIdCounter;
    public byte worldId { get; private set; }
    private PhysicsSystemSettings _settings;

    public IReadOnlyList<BodyID> bodies => _bodies;
    // public IReadOnlySet<BodyID> ignoreDrawBodies => _ignoreDrawBodies;

    protected readonly List<BodyID> _bodies = [];

    // public HashSet<BodyID> activeBodies = [];
    // public HashSet<BodyID> deactivatedBodies = [];

    // protected readonly HashSet<BodyID> _ignoreDrawBodies = [];
    public int targetFPS => TargetFPS;
    protected JobSystem jobSystem;
    public PhysicsSystem physicsSystem { get; private set; }

    protected const int TargetFPS = 60;

    private const int MaxBodies = 65536;
    private const int MaxBodyPairs = 65536;
    private const int MaxContactConstraints = 65536;
    private const int NumBodyMutexes = 0;

    // public long timestamp { get; private set; }
    // internal static class Layers
    // {
    //     public static readonly ObjectLayer NonMoving = 0;
    //     public static readonly ObjectLayer Moving = 1;
    // }

    // internal static class BroadPhaseLayers
    // {
    //     public static readonly BroadPhaseLayer NonMoving = 0;
    //     public static readonly BroadPhaseLayer Moving = 1;
    // }


    public JoltApplication()
    {
        if (!Foundation.Init(false)) return;

        Interlocked.Increment(ref _worldIdCounter);
        if (_worldIdCounter > byte.MaxValue) throw new Exception("WorldId overflow");
        worldId = (byte)_worldIdCounter;

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
        objectLayerPairFilter.EnableCollision((ushort)ObjectLayers.NonMoving, (byte)ObjectLayers.Moving);
        objectLayerPairFilter.EnableCollision((ushort)ObjectLayers.Moving, (byte)ObjectLayers.Moving);

        // We use a 1-to-1 mapping between object layers and broadphase layers
        BroadPhaseLayerInterfaceTable broadPhaseLayerInterface = new(2, 2);
        broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.NonMoving,
            (byte)BroadPhaseLayers.NonMoving);
        broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.Moving, (byte)BroadPhaseLayers.Moving);

        ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter =
            new(broadPhaseLayerInterface, 2, objectLayerPairFilter, 2);

        _settings.ObjectLayerPairFilter = objectLayerPairFilter;
        _settings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
        _settings.ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;
    }

    public BodyID CreateFloor(float size, ObjectLayer layer)
    {
        BoxShape shape = new(new Vector3(size, 5.0f, size));
        using BodyCreationSettings creationSettings =
            new(shape, new Vector3(0, -5.0f, 0.0f), Quaternion.Identity, MotionType.Static, layer);
        BodyID body = physicsSystem.BodyInterface.CreateAndAddBody(creationSettings, Activation.DontActivate);
        _bodies.Add(body);
        // _ignoreDrawBodies.Add(body);
        return body;
    }

    public BodyID Create(BodyCreationSettings settings, Activation activation = Activation.Activate)
    {
        var id = physicsSystem.BodyInterface.CreateAndAddBody(settings, activation);
        return id;
    }

    public void RemoveAndDestroy(in BodyID bodyID)
    {
        physicsSystem.BodyInterface.RemoveAndDestroyBody(bodyID);
        _bodies.Remove(bodyID);
    }

    public void Activate(in BodyID bodyID)
    {
        physicsSystem.BodyInterface.ActivateBody(bodyID);
    }

    public void Deactivate(in BodyID bodyID)
    {
        physicsSystem.BodyInterface.DeactivateBody(bodyID);
    }

    public BodyID CreateBox(in Vector3 halfExtent,
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

    public BodyID CreateSphere(float radius,
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


    public BodyID CreatePlane(
        in Vector3 position,
        in Quaternion rotation,
        in Vector3 normal,
        float distance,
        float halfExtent,
        MotionType motionType,
        ObjectLayer layer,
        PhysicsMaterial? material = null,
        Activation activation = Activation.Activate)
    {
        Plane plane = new Plane(normal, distance);
        PlaneShape shape = new(plane, material, halfExtent);
        using BodyCreationSettings creationSettings = new(shape, position, rotation, motionType, layer);
        BodyID body = physicsSystem.BodyInterface.CreateAndAddBody(creationSettings, activation);
        _bodies.Add(body);
        return body;
    }

    protected virtual ValidateResult OnContactValidate(PhysicsSystem system, in Body body1, in Body body2,
        Double3 baseOffset, in CollideShapeResult collisionResult)
    {
        // Log.Information("Contact validate callback");

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
        // ToolkitLog.Info($"A body{bodyID} got activated");
        // activeBodies.Add(bodyID);
        // if (deactivatedBodies.Contains(bodyID))
        // {
        //     deactivatedBodies.Remove(bodyID);
        // }
    }

    protected virtual void OnBodyDeactivated(PhysicsSystem system, in BodyID bodyID, ulong bodyUserData)
    {
        // ToolkitLog.Info($"A body{bodyID} got deactivated");
        // if (activeBodies.Contains(bodyID))
        // {
        //     activeBodies.Remove(bodyID);
        // }
        // deactivatedBodies.Add(bodyID);
    }

    #endregion

    #region Systems

    protected List<ISystem> systems;

    public delegate void UpdateAction(in LoopContex ctx);

    public event Action BeforeRun = delegate { };
    public event Action AfterRun = delegate { };


    public interface ISystem
    {
        public void OnAdded(JoltApplication app);


        void OnRemoved();
        public void BeforeRun();
        public void BeforeUpdate(in LoopContex ctx);
        public void AfterUpdate(in LoopContex ctx);
        public void AfterRun();

        public bool NeedShutdown()
        {
            return false;
        }

        public void Dispose();
    }


    public event UpdateAction BeforePhysicsUpdate = delegate { };
    public event UpdateAction AfterPhysicsUpdate = delegate { };


    public void AddSystem(ISystem system)
    {
        systems.Add(system);
        system.OnAdded(this);
    }

    // public void RemoveSystem(ISystem system)
    // {
    //     systems.Remove(system);
    //     system.OnRemoved();
    // }
    //
    // public bool GetSystem<T>(out T system) where T : ISystem
    // {
    //     foreach (var s in systems)
    //     {
    //         if (s is T t)
    //         {
    //             system = t;
    //             return true;
    //         }
    //     }
    //
    //     system = default!;
    //     return false;
    // }

    #endregion

    public bool running { get; private set; }

    public class LoopContex
    {
        public long CurrentFrame;
        public TimeSpan FrameBeginTimestamp;
        public TimeSpan ElapsedTimeFromPreviousFrame;
    }

    public LoopContex ctx { get; private set; }

    public void Run()
    {
        physicsSystem.OptimizeBroadPhase();
        const int collisionSteps = 1;
        float deltaTime = 1.0f / TargetFPS;
        TimeSpan deltaMs = TimeSpan.FromMilliseconds(1000 / TargetFPS);
        // using var looper = new LogicLooper(TargetFPS);


        BeforeRun();

        foreach (var system in systems)
        {
            system.BeforeRun();
        }


        running = true;
        Stopwatch stopwatch = new Stopwatch();

        ctx = new LoopContex();

        stopwatch.Start();
        while (true)
        {
            ctx.CurrentFrame++;
            ctx.FrameBeginTimestamp = stopwatch.Elapsed;

            BeforePhysicsUpdate(ctx);

            foreach (var system in systems)
            {
                system.BeforeUpdate(ctx);
            }

            PhysicsUpdateError error = physicsSystem.Update(deltaTime, collisionSteps, jobSystem);
            if (error != PhysicsUpdateError.None)
            {
                ToolkitLog.Warning($"Physics update error: {error}");
            }


            foreach (var system in systems)
            {
                system.AfterUpdate(ctx);
            }

            AfterPhysicsUpdate(ctx);

            bool needShutdown = systems.Any(s => s.NeedShutdown());

            ctx.ElapsedTimeFromPreviousFrame = stopwatch.Elapsed - ctx.FrameBeginTimestamp;

            TimeSpan sleepTime = deltaMs - ctx.ElapsedTimeFromPreviousFrame;
            if (sleepTime > TimeSpan.Zero)
            {
                Thread.Sleep(sleepTime);
            }

            if (needShutdown) break;
        }

        stopwatch.Stop();
        running = false;

        foreach (var system in systems)
        {
            system.AfterRun();
        }

        AfterRun();
    }


    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        foreach (BodyID bodyId in _bodies)
        {
            physicsSystem.BodyInterface.RemoveAndDestroyBody(bodyId);
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
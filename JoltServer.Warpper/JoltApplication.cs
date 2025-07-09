using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using GameCore.Physics;
using JoltPhysicsSharp;
using Serilog;
using UnityToolkit;
using Activation = JoltPhysicsSharp.Activation;
using MotionType = JoltPhysicsSharp.MotionType;

namespace JoltServer;

public class JoltApplication : DisposableObject
{
    // public HashSet<BodyID> activeBodies = [];
    // public HashSet<BodyID> deactivatedBodies = [];

    // protected readonly HashSet<BodyID> _ignoreDrawBodies = [];
    public int targetFPS => TargetFPS;
    public JoltPhysicsWorld physicsWorld { get; private set; }
    protected const int TargetFPS = 60;
    // protected const int WorldHistoryLength = 128;

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
        physicsWorld = new JoltPhysicsWorld(SetupCollisionFiltering);

        systems = new List<IJoltSystem<JoltApplication, LoopContex,JoltPhysicsWorld>>();
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
    }

    #region Physics

    protected static void SetupCollisionFiltering(ref PhysicsSystemSettings settings)
    {
        // We use only 2 layers: one for non-moving objects and one for moving objects
        ObjectLayerPairFilterTable objectLayerPairFilter = new(2);
        objectLayerPairFilter.EnableCollision((uint)ObjectLayers.NonMoving, (uint)ObjectLayers.Moving);
        objectLayerPairFilter.EnableCollision((uint)ObjectLayers.Moving, (uint)ObjectLayers.Moving);

        // We use a 1-to-1 mapping between object layers and broadphase layers
        BroadPhaseLayerInterfaceTable broadPhaseLayerInterface = new(2, 2);
        broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.NonMoving,
            (byte)BroadPhaseLayers.NonMoving);
        broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((ushort)ObjectLayers.Moving, (byte)BroadPhaseLayers.Moving);

        ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter =
            new(broadPhaseLayerInterface, 2, objectLayerPairFilter, 2);

        settings.ObjectLayerPairFilter = objectLayerPairFilter;
        settings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
        settings.ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;
    }

    // public BodyID CreateFloor(float size, ObjectLayer layer)
    // {
    //     BoxShape shape = new(new Vector3(size, 5.0f, size));
    //     using BodyCreationSettings creationSettings =
    //         new(shape, new Vector3(0, -5.0f, 0.0f), Quaternion.Identity, MotionType.Static, layer);
    //     return Create(creationSettings, Activation.DontActivate);
    // }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public BodyID Create(BodyCreationSettings settings, Activation activation = Activation.Activate) =>
    //     physicsWorld.Create(settings, activation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAndDestroy(in BodyID bodyID) => physicsWorld.RemoveAndDestroy(bodyID);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Activate(in BodyID bodyID) => physicsWorld.Activate(bodyID);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deactivate(in BodyID bodyID) => physicsWorld.Deactivate(bodyID);

    // public BodyID CreateBox(in Vector3 halfExtent,
    //     in Vector3 position,
    //     in Quaternion rotation,
    //     MotionType motionType,
    //     ObjectLayer layer,
    //     Activation activation = Activation.Activate)
    // {
    //     BoxShape shape = new(halfExtent);
    //     using BodyCreationSettings creationSettings = new(shape, position, rotation, motionType, layer);
    //     return Create(creationSettings, activation);
    // }

    // public BodyID CreateSphere(float radius,
    //     in Vector3 position,
    //     in Quaternion rotation,
    //     MotionType motionType,
    //     ObjectLayer layer,
    //     Activation activation = Activation.Activate)
    // {
    //     SphereShape shape = new(radius);
    //     using BodyCreationSettings creationSettings = new(shape, position, rotation, motionType, layer);
    //     return Create(creationSettings, activation);
    // }


    // public BodyID CreatePlane(
    //     in Vector3 position,
    //     in Quaternion rotation,
    //     in Vector3 normal,
    //     float distance,
    //     float halfExtent,
    //     MotionType motionType,
    //     ObjectLayer layer,
    //     PhysicsMaterial? material = null,
    //     Activation activation = Activation.Activate)
    // {
    //     Plane plane = new Plane(normal, distance);
    //     PlaneShape shape = new(plane, material, halfExtent);
    //     using BodyCreationSettings creationSettings = new(shape, position, rotation, motionType, layer);
    //     return Create(creationSettings, activation);
    // }

    #endregion

    #region Systems

    protected List<IJoltSystem<JoltApplication, LoopContex,JoltPhysicsWorld>> systems;

    public class LoopContex
    {
        public long CurrentFrame;
        public TimeSpan FrameBeginTimestamp;
        public TimeSpan ElapsedTimeFromPreviousFrame;
    }

    public event Action BeforeStart = delegate { };
    public event Action AfterPhysicsStop = delegate { };


    public delegate void UpdateAction(in LoopContex ctx);

    public event UpdateAction BeforePhysicsUpdate = delegate { };
    public event UpdateAction AfterPhysicsUpdate = delegate { };


    public void AddSystem(IJoltSystem<JoltApplication, LoopContex, JoltPhysicsWorld> system)
    {
        systems.Add(system);
        system.OnAdded(this, physicsWorld);
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


    public LoopContex ctx { get; private set; }

    public void Run()
    {
        physicsWorld.physicsSystem.OptimizeBroadPhase();
        const int collisionSteps = 1;
        float deltaTime = 1.0f / TargetFPS;
        TimeSpan deltaMs = TimeSpan.FromMilliseconds(1000 / TargetFPS);
        // using var looper = new LogicLooper(TargetFPS);


        BeforeStart();

        foreach (var system in systems)
        {
            system.BeforePhysicsStart();
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
                system.BeforePhysicsUpdate(ctx);
            }

            var error = physicsWorld.Simulate(deltaTime, collisionSteps);
            if (error != GameCore.Physics.PhysicsUpdateError.None)
            {
                ToolkitLog.Warning($"Physics update error: {error}");
            }


            foreach (var system in systems)
            {
                system.AfterPhysicsUpdate(ctx);
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
            system.AfterPhysicsStop();
        }

        AfterPhysicsStop();
    }


    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        physicsWorld.Dispose();
        foreach (var system in systems)
        {
            system.Dispose();
        }

        systems.Clear();
        Foundation.Shutdown();
    }
}
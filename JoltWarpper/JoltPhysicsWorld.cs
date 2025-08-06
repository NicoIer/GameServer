using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using GameCore.Physics;
using JoltPhysicsSharp;
using Serilog;
using UnityToolkit;
using ShapeSubType = JoltPhysicsSharp.ShapeSubType;

namespace JoltServer;

public class JoltPhysicsWorld : IPhysicsWorld
{
    public byte worldId { get; private set; }

    // public readonly Dictionary<uint, int> body2Owner = new Dictionary<uint, int>();


    public IReadOnlyList<uint> bodies => _bodies;
    // public IReadOnlySet<BodyID> ignoreDrawBodies => _ignoreDrawBodies;

    protected readonly List<uint> _bodies = new();
    protected readonly JobSystem jobSystem = new JobSystemThreadPool();
    public PhysicsSystem physicsSystem { get; private set; } = null!;

    // public float time { get; private set; }
    // public long frame { get; private set; }

    // private LinkedList<WorldData> history = new LinkedList<WorldData>();
    // private int historyBufferSize;

    // public delegate void SetupCollisionFilteringDelegate(ref PhysicsSystemSettings settings);

    public JoltPhysicsWorld()
        // , int historyBufferSize)
    {
        if (!Foundation.Init(false)) throw new Exception("Jolt Physics Not Initialized");
        // history = new LinkedList<WorldData>();
        // this.historyBufferSize = historyBufferSize;
        Interlocked.Increment(ref IPhysicsWorld.worldIdCounter);
        if (IPhysicsWorld.worldIdCounter > byte.MaxValue) throw new Exception("WorldId overflow");
        worldId = (byte)IPhysicsWorld.worldIdCounter;

        #region Setup

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

        var settings = new PhysicsSystemSettings
        {
            MaxBodies = IPhysicsWorld.MaxBodies,
            MaxBodyPairs = IPhysicsWorld.MaxBodyPairs,
            MaxContactConstraints = IPhysicsWorld.MaxContactConstraints,
            ObjectLayerPairFilter = objectLayerPairFilter,
            BroadPhaseLayerInterface = broadPhaseLayerInterface,
            ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter,
        };

        #endregion


        settings.ObjectLayerPairFilter = objectLayerPairFilter;
        settings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
        settings.ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;

        physicsSystem = new PhysicsSystem(settings);

        // ContactListener
        physicsSystem.OnContactValidate += OnContactValidate;
        physicsSystem.OnContactAdded += OnContactAdded;
        physicsSystem.OnContactPersisted += OnContactPersisted;
        physicsSystem.OnContactRemoved += OnContactRemoved;
        // BodyActivationListener
        physicsSystem.OnBodyActivated += OnBodyActivated;
        physicsSystem.OnBodyDeactivated += OnBodyDeactivated;
    }


    #region Callback

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameCore.Physics.PhysicsUpdateError Simulate(in float deltaTime, in int collisionSteps)
    {
        // if (history.Count == historyBufferSize)
        // {
        //     // Remove Head
        //     history.RemoveFirst();
        // }
        //
        // Serialize(out var data);
        // //Append Tail
        // history.AddLast(data);

        return (GameCore.Physics.PhysicsUpdateError)physicsSystem.Update(deltaTime, collisionSteps, jobSystem);
    }


    // internal BodyID Create(BodyCreationSettings settings, JoltPhysicsSharp.Activation activation)
    // {
    //     var body = physicsSystem.BodyInterface.CreateAndAddBody(settings, activation);
    //     OnBodyCreated(body);
    //     return body;
    // }
    public uint CreateAndAdd(IShapeData shapeData, in Vector3 position, in Quaternion rotation,
        GameCore.Physics.MotionType motionType,
        ObjectLayers layers, GameCore.Physics.Activation activation)
    {
        Shape? shape;
        switch (shapeData)
        {
            case BoxShapeData boxShapeData:
                shape = new BoxShape(boxShapeData.halfExtents);
                break;
            case SphereShapeData sphereShapeData:
                shape = new SphereShape(sphereShapeData.radius);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(shapeData));
        }

        if (shape == null)
        {
            throw new ArgumentException($"cannot create shape settings from shape[{shapeData}] ");
        }

        using var bodyCreate = new BodyCreationSettings(
            shape,
            position,
            rotation,
            (JoltPhysicsSharp.MotionType)motionType,
            new ObjectLayer((uint)layers)
        );
        var body = physicsSystem.BodyInterface.CreateAndAddBody(bodyCreate,
            (JoltPhysicsSharp.Activation)activation); // TODO Create Add Body
        _bodies.Add(body.ID);
        return body.ID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsAdded(in uint id)
    {
        return physicsSystem.BodyInterface.IsAdded(id);
    }


    public bool QueryBody(in uint id, [UnscopedRef] out BodyData bodyData)
    {
        var bodyInterface = physicsSystem.BodyInterface;
        var bodyId = new BodyID(id);
        Debug.Assert(bodyInterface.IsAdded(bodyId));
        // bool isSensor = false;
        ShapeDataPacket? shapeDataPacket = null;

        // physicsSystem.BodyLockInterface.LockRead(id, out var @lock);
        // Debug.Assert(@lock.Succeeded);
        // if (@lock.Succeeded)
        // {
        //     Debug.Assert(@lock.Body != null);
        //     isSensor = @lock.Body.IsSensor;
        // }
        //
        // physicsSystem.BodyLockInterface.UnlockRead(@lock);

        var shape = bodyInterface.GetShape(bodyId);
        Debug.Assert(shape != null);
        if (PackShapeData(shape, out var packet))
        {
            shapeDataPacket = packet;
        }


        // var ownerId = body2Owner.GetValueOrDefault(id, IPhysicsWorld.ServerId);

        Vector3 position = bodyInterface.GetPosition(bodyId);
        Quaternion rotation = bodyInterface.GetRotation(bodyId);


        bodyData = new BodyData()
        {
            // ownerId = ownerId,
            id = id,
            bodyType = (GameCore.Physics.BodyType)bodyInterface.GetBodyType(bodyId),
            isActive = bodyInterface.IsActive(bodyId),
            motionType = (GameCore.Physics.MotionType)bodyInterface.GetMotionType(bodyId),
            // isSensor = isSensor,
            objectLayer = bodyInterface.GetObjectLayer(bodyId),
            friction = bodyInterface.GetFriction(bodyId),
            restitution = bodyInterface.GetRestitution(bodyId),
            position = position,
            rotation = rotation,
            centerOfMass = bodyInterface.GetCenterOfMassPosition(bodyId),
            linearVelocity = bodyInterface.GetLinearVelocity(bodyId),
            angularVelocity = bodyInterface.GetAngularVelocity(bodyId),
            shapeDataPacket = shapeDataPacket
        };


        if (shapeDataPacket == null)
        {
            ToolkitLog.Info($"got a null shape packet,id:{id},threadId:{Thread.CurrentThread.ManagedThreadId}");
        }

        return true;
    }

    public Vector3 GetPosition(in uint id)
    {
        return physicsSystem.BodyInterface.GetPosition(id);
    }

    public Quaternion GetRotation(in uint id)
    {
        return physicsSystem.BodyInterface.GetRotation(id);
    }

    private static bool PackShapeData(in Shape shape, out ShapeDataPacket packet)
    {
        throw new NotImplementedException("PackShapeData method is not implemented yet");
    }


    public void Serialize(ref WorldData worldData)
    {
        Debug.Assert(worldData.bodies.Count >= bodies.Count);
        worldData.gravity = physicsSystem.Gravity;


        for (var i = 0; i < bodies.Count; i++)
        {
            var id = bodies[i];

            Debug.Assert(physicsSystem.BodyInterface.IsAdded(new BodyID(id)),
                $"body {id} is not added to the physics world");
            QueryBody(id, out var data);
            worldData.bodies[i] = data;


            Debug.Assert(float.IsNaN(worldData.bodies[i].position.X) == false);
            Debug.Assert(float.IsNaN(worldData.bodies[i].position.Y) == false);
            Debug.Assert(float.IsNaN(worldData.bodies[i].position.Z) == false);

            Debug.Assert(float.IsNaN(worldData.bodies[i].rotation.X) == false);
            Debug.Assert(float.IsNaN(worldData.bodies[i].rotation.Y) == false);
            Debug.Assert(float.IsNaN(worldData.bodies[i].rotation.Z) == false);
            Debug.Assert(float.IsNaN(worldData.bodies[i].rotation.W) == false);
        }
    }

    public void Deserialize(in WorldData worldData)
    {
        throw new NotImplementedException();
    }

    public void Activate(in uint id)
    {
        physicsSystem.BodyInterface.ActivateBody(id);
    }

    public void Deactivate(in uint id)
    {
        physicsSystem.BodyInterface.DeactivateBody(id);
    }

    public void RemoveAndDestroy(in uint id)
    {
        physicsSystem.BodyInterface.RemoveAndDestroyBody(id);
        _bodies.Remove(id);
        // body2Owner.Remove(id);
    }

    public void Dispose()
    {
        foreach (BodyID bodyId in _bodies)
        {
            physicsSystem.BodyInterface.RemoveAndDestroyBody(bodyId);
        }

        _bodies.Clear();
        jobSystem.Dispose();
        physicsSystem.Dispose();
    }
}
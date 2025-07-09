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
    protected readonly JobSystem jobSystem = null!;
    public PhysicsSystem physicsSystem { get; private set; } = null!;

    // public float time { get; private set; }
    // public long frame { get; private set; }

    // private LinkedList<WorldData> history = new LinkedList<WorldData>();
    // private int historyBufferSize;

    public delegate void SetupCollisionFilteringDelegate(ref PhysicsSystemSettings settings);

    public JoltPhysicsWorld(SetupCollisionFilteringDelegate setup)
        // , int historyBufferSize)
    {
        if (!Foundation.Init(false)) throw new Exception("Jolt Physics Not Initialized");
        // history = new LinkedList<WorldData>();
        // this.historyBufferSize = historyBufferSize;
        Interlocked.Increment(ref IPhysicsWorld.worldIdCounter);
        if (IPhysicsWorld.worldIdCounter > byte.MaxValue) throw new Exception("WorldId overflow");
        worldId = (byte)IPhysicsWorld.worldIdCounter;

        var settings = new PhysicsSystemSettings()
        {
            MaxBodies = IPhysicsWorld.MaxBodies,
            MaxBodyPairs = IPhysicsWorld.MaxBodyPairs,
            MaxContactConstraints = IPhysicsWorld.MaxContactConstraints,
            NumBodyMutexes = IPhysicsWorld.NumBodyMutexes,
        };
        jobSystem = new JobSystemThreadPool();
        setup(ref settings);
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
            case PlaneShapeData planeShapeData:
                Plane plane = new Plane(planeShapeData.normal, planeShapeData.distance);
                shape = new PlaneShape(plane, null, planeShapeData.halfExtent);
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
        var body = physicsSystem.BodyInterface.CreateAndAddBody(bodyCreate, (JoltPhysicsSharp.Activation)activation); // TODO Create Add Body
        OnBodyCreated(body.ID);
        return body.ID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exist(in uint id)
    {
        return physicsSystem.BodyInterface.IsAdded(id);
    }
    // public uint Create(IShapeData shape, in Vector3 position, in Quaternion rotation,
    //     GameCore.Jolt.MotionType motionType,
    //     ObjectLayers layers, GameCore.Jolt.Activation activation)
    // {
    //     ShapeSettings? shapeSettings;
    //     switch (shape)
    //     {
    //         case BoxShapeData boxShapeData:
    //             shapeSettings = new BoxShapeSettings(boxShapeData.halfExtents);
    //             break;
    //         case PlaneShapeData planeShapeData:
    //             Plane plane = new Plane(planeShapeData.normal, planeShapeData.distance);
    //             shapeSettings = new PlaneShapeSettings(plane, null, planeShapeData.halfExtent);
    //             break;
    //         case SphereShapeData sphereShapeData:
    //             shapeSettings = new SphereShapeSettings(sphereShapeData.radius);
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(shape));
    //     }
    //
    //     if (shapeSettings == null)
    //     {
    //         throw new ArgumentException($"cannot create shape settings from shape[{shape}] ");
    //     }
    //
    //     var bodyCreate = new BodyCreationSettings(
    //         shapeSettings,
    //         position,
    //         rotation,
    //         (JoltPhysicsSharp.MotionType)motionType,
    //         new ObjectLayer((uint)layers)
    //     );
    //     var body = physicsSystem.BodyInterface.CreateAndAddBody(bodyCreate, (JoltPhysicsSharp.Activation)activation);
    //     OnBodyCreated(body);
    //     return body.ID;
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnBodyCreated(in uint bodyId, int owner = IPhysicsWorld.ServerId)
    {
        _bodies.Add(bodyId);
        // body2Owner.Add(bodyId, owner);
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
        packet = default;
        switch (shape.SubType)
        {
            case ShapeSubType.Sphere:
                break;
            case ShapeSubType.Box:
                JoltApi.JPH_BoxShape_GetHalfExtent(shape.Handle, out Vector3 value);
                var box = new BoxShapeData(value);
                ShapeDataPacket.Create(box, out packet);
                break;
            case ShapeSubType.Triangle:
                break;
            case ShapeSubType.Capsule:
                break;
            case ShapeSubType.TaperedCapsule:
                break;
            case ShapeSubType.Cylinder:
                break;
            case ShapeSubType.ConvexHull:
                break;
            case ShapeSubType.StaticCompound:
                break;
            case ShapeSubType.MutableCompound:
                break;
            case ShapeSubType.RotatedTranslated:
                break;
            case ShapeSubType.Scaled:
                break;
            case ShapeSubType.OffsetCenterOfMass:
                break;
            case ShapeSubType.Mesh:
                break;
            case ShapeSubType.HeightField:
                break;
            case ShapeSubType.SoftBody:
                break;
            case ShapeSubType.User1:
                break;
            case ShapeSubType.User2:
                break;
            case ShapeSubType.User3:
                break;
            case ShapeSubType.User4:
                break;
            case ShapeSubType.User5:
                break;
            case ShapeSubType.User6:
                break;
            case ShapeSubType.User7:
                break;
            case ShapeSubType.User8:
                break;
            case ShapeSubType.UserConvex1:
                break;
            case ShapeSubType.UserConvex2:
                break;
            case ShapeSubType.UserConvex3:
                break;
            case ShapeSubType.UserConvex4:
                break;
            case ShapeSubType.UserConvex5:
                break;
            case ShapeSubType.UserConvex6:
                break;
            case ShapeSubType.UserConvex7:
                break;
            case ShapeSubType.UserConvex8:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    public bool UpdateBody(in uint id, in BodyData bodyData)
    {
        throw new NotImplementedException();
    }


    public void Serialize(ref WorldData worldData)
    {
        Debug.Assert(worldData.bodies.Count >= bodies.Count);
        worldData.gravity = physicsSystem.Gravity;


        for (var i = 0; i < bodies.Count; i++)
        {
            var id = bodies[i];
            Debug.Assert(physicsSystem.BodyInterface.IsAdded(id));
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
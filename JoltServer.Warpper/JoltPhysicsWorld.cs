using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using GameCore.Jolt;
using JoltPhysicsSharp;
using Serilog;
using UnityToolkit;
using Activation = JoltPhysicsSharp.Activation;
using PhysicsUpdateError = GameCore.Jolt.PhysicsUpdateError;

namespace JoltServer;

public class JoltPhysicsWorld : IPhysicsWorld
{
    public byte worldId { get; private set; }

    public readonly Dictionary<uint, int> body2Owner = new Dictionary<uint, int>();
    public const int ServerId = 0;

    public IReadOnlyList<BodyID> bodies => _bodies;
    // public IReadOnlySet<BodyID> ignoreDrawBodies => _ignoreDrawBodies;

    protected readonly List<BodyID> _bodies = [];
    private PhysicsSystemSettings _settings;
    protected JobSystem jobSystem;
    public PhysicsSystem physicsSystem { get; private set; }

    private const int MaxBodies = 65536;
    private const int MaxBodyPairs = 65536;
    private const int MaxContactConstraints = 65536;
    private const int NumBodyMutexes = 0;

    // private LinkedList<WorldData> history = new LinkedList<WorldData>();
    // private int historyBufferSize;

    public delegate void SetupCollisionFilteringDelegate(ref PhysicsSystemSettings settings);

    public JoltPhysicsWorld(SetupCollisionFilteringDelegate setup)
        // , int historyBufferSize)
    {
        if (!Foundation.Init(false)) return;
        // history = new LinkedList<WorldData>();
        // this.historyBufferSize = historyBufferSize;
        Interlocked.Increment(ref IPhysicsWorld.worldIdCounter);
        if (IPhysicsWorld.worldIdCounter > byte.MaxValue) throw new Exception("WorldId overflow");
        worldId = (byte)IPhysicsWorld.worldIdCounter;

        _settings = new PhysicsSystemSettings()
        {
            MaxBodies = MaxBodies,
            MaxBodyPairs = MaxBodyPairs,
            MaxContactConstraints = MaxContactConstraints,
            NumBodyMutexes = NumBodyMutexes,
        };
        jobSystem = new JobSystemThreadPool();
        setup(ref _settings);
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

    public BodyID Create(BodyCreationSettings settings, Activation activation)
    {
        var id = physicsSystem.BodyInterface.CreateAndAddBody(settings, activation);
        _bodies.Add(id);
        body2Owner.Add(id, ServerId);
        return id;
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

    public PhysicsUpdateError Simulate(float deltaTime, int collisionSteps)
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

        return (PhysicsUpdateError)physicsSystem.Update(deltaTime, collisionSteps, jobSystem);
    }

    public bool QueryBody(in uint id, [UnscopedRef] out BodyData bodyData)
    {
        Debug.Assert(physicsSystem.BodyInterface.IsAdded(id));

        bool isSensor = false;
        ShapeDataPacket? shapeDataPacket = null;

        physicsSystem.BodyLockInterface.LockRead(id, out var @lock);
        Debug.Assert(@lock.Succeeded);
        if (@lock.Succeeded)
        {
            Debug.Assert(@lock.Body != null);
            isSensor = @lock.Body.IsSensor;
        }

        physicsSystem.BodyLockInterface.UnlockRead(@lock);

        var shape = physicsSystem.BodyInterface.GetShape(id);
        Debug.Assert(shape != null);
        if (PackShapeData(shape, out var packet))
        {
            shapeDataPacket = packet;
        }


        var ownerId = body2Owner.GetValueOrDefault(id, ServerId);

        Vector3 position = physicsSystem.BodyInterface.GetPosition(id);
        Quaternion rotation = physicsSystem.BodyInterface.GetRotation(id);


        bodyData = new BodyData()
        {
            ownerId = ownerId,
            entityId = id,
            bodyType = (GameCore.Jolt.BodyType)physicsSystem.BodyInterface.GetBodyType(id),
            isActive = physicsSystem.BodyInterface.IsActive(id),
            motionType = (GameCore.Jolt.MotionType)physicsSystem.BodyInterface.GetMotionType(id),
            isSensor = isSensor,
            objectLayer = physicsSystem.BodyInterface.GetObjectLayer(id),
            friction = physicsSystem.BodyInterface.GetFriction(id),
            restitution = physicsSystem.BodyInterface.GetRestitution(id),
            position = position,
            rotation = rotation,
            centerOfMass = physicsSystem.BodyInterface.GetCenterOfMassPosition(id),
            linearVelocity = physicsSystem.BodyInterface.GetLinearVelocity(id),
            angularVelocity = physicsSystem.BodyInterface.GetAngularVelocity(id),
            shapeDataPacket = shapeDataPacket
        };


        if (shapeDataPacket == null)
        {
            ToolkitLog.Info($"got a null shape packet,id:{id},threadId:{Thread.CurrentThread.ManagedThreadId}");
        }

        return true;
    }

    private static bool PackShapeData(in Shape shape, out ShapeDataPacket packet)
    {
        packet = default;
        switch (shape)
        {
            case MutableCompoundShape mutableCompoundShape:
                break;
            case StaticCompoundShape staticCompoundShape:
                break;
            case CompoundShape compoundShape:
                break;
            case CapsuleShape capsuleShape:
                break;
            case BoxShape boxShape:
                var box = new BoxShapeData(boxShape.HalfExtent);
                ShapeDataPacket.Create(box, out packet);
                break;
            case ConvexHullShape convexHullShape:
                break;
            case CylinderShape cylinderShape:
                break;
            case OffsetCenterOfMassShape offsetCenterOfMassShape:
                break;
            case SphereShape sphereShape:
                var sphere = new SphereShapeData(sphereShape.Radius);
                ShapeDataPacket.Create(sphere, out packet);
                break;
            case TaperedCapsuleShape taperedCapsuleShape:
                break;
            case TaperedCylinderShape taperedCylinderShape:
                break;
            case TriangleShape triangleShape:
                break;
            case ConvexShape convexShape:
                break;
            case RotatedTranslatedShape rotatedTranslatedShape:
                break;
            case ScaledShape scaledShape:
                break;
            case DecoratedShape decoratedShape:
                break;
            case EmptyShape emptyShape:
                break;
            case HeightFieldShape heightFieldShape:
                break;
            case MeshShape meshShape:
                break;
            case PlaneShape planeShape:
                var plane = new PlaneShapeData
                {
                    halfExtent = planeShape.HalfExtent,
                    normal = planeShape.Plane.Normal,
                    distance = planeShape.Plane.D,
                };
                ShapeDataPacket.Create(plane, out packet);
                break;
            default:
                // TODO 额外的处理逻辑
                if (shape is { Type: JoltPhysicsSharp.ShapeType.Convex, SubType: JoltPhysicsSharp.ShapeSubType.Box })
                {
                    // Log.Warning("异常Shape进行额外处理，解析为BoxShape");
                    ShapeDataPacket.Create(new BoxShapeData(shape.LocalBounds.Extent / 2), out packet);
                    return true;
                }

                Log.Warning("Shape:{shape}序列化异常,{type},{subType}", shape, shape.Type, shape.SubType);
                return false;
        }

        return true;
    }

    public bool UpdateBody(in uint id, in BodyData bodyData)
    {
        throw new NotImplementedException();
    }

    public bool QueryHistoryData(byte delta, [UnscopedRef] out WorldData worldData)
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
        body2Owner.Remove(id);
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
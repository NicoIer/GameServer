using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cysharp.Threading;
using GameCore.Jolt;
using JoltPhysicsSharp;
using Network;
using Network.Server;
using Network.Telepathy;
using Serilog;
using UnityToolkit;
using MotionType = JoltPhysicsSharp.MotionType;
using ShapeSubType = JoltPhysicsSharp.ShapeSubType;

namespace JoltServer;

// TODO 将复制物理世界 网络传输部分 在另一个线程执行
public partial class JoltServer : JoltApplication.ISystem
{
    public const int ServerId = 0;

    private JoltApplication _app;

    private readonly LogicLooper _networkLooper;

    public int targetFrameRate { get; private set; }

    private readonly NetworkServer _server;

    /// <summary>
    /// 上一次发送的世界帧
    /// </summary>
    private long _lastSendWorldFrame = -1;

    /// <summary>
    /// 世界信息快照
    /// </summary>
    private readonly CircularBuffer<WorldData> _worldSnapshot;

    private readonly Dictionary<uint, int> _body2Owner = new Dictionary<uint, int>();
    // private readonly Dictionary<int, uint> _owner2Body = new Dictionary<int, uint>();

    public JoltServer(int targetFrameRate, int port, int bufferSize = 10)
    {
        _worldSnapshot = new CircularBuffer<WorldData>(bufferSize);
        _worldSnapshot.OnRemove += (in WorldData data) =>
        {
            // Log.Information($"Remove WorldData {data.frameCount} Return Bodies Array");
            Debug.Assert(data.bodies.Array != null);
            ArrayPool<BodyData>.Shared.Return(data.bodies.Array);
        };
        if (port > ushort.MaxValue)
        {
            throw new ArgumentException("Port must be less than or equal to 65535");
        }

        this.targetFrameRate = targetFrameRate;
        _networkLooper = new LogicLooper(targetFrameRate);
        _server = new NetworkServer(new TelepathyServerSocket((ushort)port), (ushort)targetFrameRate, true);

        HandleCmd();
        HandleReqRsp();
    }


    public void OnAdded(JoltApplication app)
    {
        _app = app;
    }

    public void OnRemoved()
    {
    }


    public void BeforeRun()
    {
        Log.Information($"JoltServer Start {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        _server.Run(false);
        // _server.Run(true);
        _networkLooper.RegisterActionAsync((in LogicLooperActionContext ctx) =>
        {
            ref var worldData = ref _worldSnapshot.buffer[_worldSnapshot.backIndex];
            if (worldData.frameCount != _lastSendWorldFrame)
            {
                _server.SendToAll(worldData);
                _lastSendWorldFrame = worldData.frameCount;
            }

            return true;
        });
    }


    public void BeforeUpdate(in JoltApplication.LoopContex ctx)
    {
        _server.socket.TickIncoming();
    }

    private bool PackShapeData(in Shape shape, out ShapeDataPacket packet)
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
                return false;
        }

        return true;
    }

    public void PackBodyData(in BodyID id, out BodyData data)
    {
        Debug.Assert(_app.physicsSystem.BodyInterface.IsAdded(id));
        var shape = _app.physicsSystem.BodyInterface.GetShape(id);
        Debug.Assert(shape != null);
        ShapeDataPacket? shapeDataPacket = null;
        if (PackShapeData(shape, out var packet))
        {
            shapeDataPacket = packet;
        }



        var ownerId = _body2Owner.GetValueOrDefault(id, ServerId);

        Vector3 position = _app.physicsSystem.BodyInterface.GetPosition(id);
        Quaternion rotation = _app.physicsSystem.BodyInterface.GetRotation(id);


        _app.physicsSystem.BodyLockInterface.LockRead(id, out var @lock);

        bool isSensor = false;
        if (@lock.Succeeded)
        {
            Debug.Assert(@lock.Body != null);
            isSensor = @lock.Body.IsSensor;
        }

        _app.physicsSystem.BodyLockInterface.UnlockRead(@lock);

        data = new BodyData()
        {
            ownerId = ownerId,
            entityId = id.ID,
            bodyType = (GameCore.Jolt.BodyType)_app.physicsSystem.BodyInterface.GetBodyType(id),
            isActive = _app.physicsSystem.BodyInterface.IsActive(id),
            motionType = (GameCore.Jolt.MotionType)_app.physicsSystem.BodyInterface.GetMotionType(id),
            isSensor = isSensor,
            objectLayer = _app.physicsSystem.BodyInterface.GetObjectLayer(id),
            friction = _app.physicsSystem.BodyInterface.GetFriction(id),
            restitution = _app.physicsSystem.BodyInterface.GetRestitution(id),
            position = position,
            rotation = rotation,
            centerOfMass = _app.physicsSystem.BodyInterface.GetCenterOfMassPosition(id),
            linearVelocity = _app.physicsSystem.BodyInterface.GetLinearVelocity(id),
            angularVelocity = _app.physicsSystem.BodyInterface.GetAngularVelocity(id),
            shapeDataPacket = shapeDataPacket
        };
        
        
        if (shapeDataPacket == null)
        {
            ToolkitLog.Info($"获得了一个Null的ShapeDataPacket,id:{id},{JsonSerializer.Serialize(data)}");
        }
    }

    public unsafe void AfterUpdate(in JoltApplication.LoopContex ctx)
    {
        // Console.WriteLine($"AfterUpdate {ctx.CurrentFrame}");
        BodyData[] array = ArrayPool<BodyData>.Shared.Rent(_app.bodies.Count);
        var bodies = new ArraySegment<BodyData>(array, 0, _app.bodies.Count);
        WorldData worldData;
        worldData.bodies = bodies;
        // worldData.worldId = _app.worldId;
        worldData.gravity = _app.physicsSystem.Gravity;
        worldData.timeStamp = (long)_app.ctx.FrameBeginTimestamp.TotalMicroseconds;
        worldData.frameCount = _app.ctx.CurrentFrame;


        for (var i = 0; i < _app.bodies.Count; i++)
        {
            var id = _app.bodies[i];
            Debug.Assert(_app.physicsSystem.BodyInterface.IsAdded(id));
            PackBodyData(id, out var data);
            bodies[i] = data;


            Debug.Assert(float.IsNaN(bodies[i].position.X) == false);
            Debug.Assert(float.IsNaN(bodies[i].position.Y) == false);
            Debug.Assert(float.IsNaN(bodies[i].position.Z) == false);

            Debug.Assert(float.IsNaN(bodies[i].rotation.X) == false);
            Debug.Assert(float.IsNaN(bodies[i].rotation.Y) == false);
            Debug.Assert(float.IsNaN(bodies[i].rotation.Z) == false);
            Debug.Assert(float.IsNaN(bodies[i].rotation.W) == false);
        }


        _worldSnapshot.PushBack(worldData);

        _server.socket.TickOutgoing();
    }


    public void AfterRun()
    {
        _server.Stop();
        _networkLooper.ShutdownAsync(TimeSpan.Zero).Wait();
    }


    public void Dispose()
    {
    }
}
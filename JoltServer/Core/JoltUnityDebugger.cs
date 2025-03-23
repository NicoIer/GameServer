using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Cysharp.Threading;
using GameCore.Jolt;
using JoltPhysicsSharp;
using Network;
using Network.Server;
using Network.Telepathy;
using Serilog;
using UnityToolkit;

namespace JoltServer;

// TODO 将复制物理世界 网络传输部分 在另一个线程执行
public class JoltUnityDebugger : JoltApplication.ISystem
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

    public JoltUnityDebugger(int targetFrameRate, int port, int bufferSize = 10)
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

        _server.messageHandler.Add<CmdSpawnBox>(OnCmdSpawnBox);
        _server.messageHandler.Add<CmdSpawnPlane>(OnCmdSpawnPlane);
        _server.messageHandler.Add<CmdDestroy>(OnCmdDestroy);
        _server.messageHandler.Add<CmdBodyState>(OnCmdBodyState);
    }


    // public ConcurrentQueue<INetworkMessage> networkMessages;

    private void OnCmdBodyState(in int connectionid, in CmdBodyState message)
    {
        Log.Information($"客户端{connectionid}请求更新Body:{message.entityId}");
        if (!_body2Owner.ContainsKey(message.entityId))
        {
            Log.Warning($"客户端{connectionid}请求更新不存在的Body:{message.entityId}");
            return;
        }

        if (_body2Owner[message.entityId] != connectionid)
        {
            Log.Warning($"客户端{connectionid}请求更新不属于自己的Body:{message.entityId}由{_body2Owner[message.entityId]}所有");
            return;
        }

        BodyID bodyId = new BodyID(message.entityId);

        if (message.position != null)
        {
            _app.physicsSystem.BodyInterface.SetPosition(bodyId, message.position.Value,
                (JoltPhysicsSharp.Activation)message.activation);
        }

        if (message.rotation != null)
        {
            _app.physicsSystem.BodyInterface.SetRotation(bodyId, message.rotation.Value,
                (JoltPhysicsSharp.Activation)message.activation);
        }

        if (message.linearVelocity != null)
        {
            _app.physicsSystem.BodyInterface.SetLinearVelocity(bodyId, message.linearVelocity.Value);
        }

        if (message.angularVelocity != null)
        {
            _app.physicsSystem.BodyInterface.SetAngularVelocity(bodyId, message.angularVelocity.Value);
        }

        var active = _app.physicsSystem.BodyInterface.IsActive(bodyId);
        if (active == message.isActive) return;
        if (message.isActive)
        {
            _app.physicsSystem.BodyInterface.ActivateBody(bodyId);
        }
        else
        {
            _app.physicsSystem.BodyInterface.DeactivateBody(bodyId);
        }
    }

    private void OnCmdDestroy(in int connectionid, in CmdDestroy message)
    {
        Log.Information($"客户端{connectionid}请求销毁Body:{message.entityId}");

        if (!_body2Owner.ContainsKey(message.entityId))
        {
            Log.Warning($"客户端{connectionid}请求销毁不存在的Body:{message.entityId}");
            return;
        }

        if (_body2Owner[message.entityId] != connectionid)
        {
            Log.Warning($"客户端{connectionid}请求销毁不属于自己的Body:{message.entityId}由{_body2Owner[message.entityId]}所有");
            return;
        }

        _app.RemoveAndDestroy(message.entityId);
        _body2Owner.Remove(message.entityId);


        Log.Information($"销毁Body成功:{message.entityId}");
    }


    private void OnCmdSpawnPlane(in int connectionid, in CmdSpawnPlane message)
    {
        Log.Information($"客户端{connectionid}请求生成Plane");
        var bodyId = _app.CreatePlane(
            message.position,
            message.rotation,
            message.normal,
            message.distance,
            message.halfExtent,
            (JoltPhysicsSharp.MotionType)message.motionType,
            (ushort)message.objectLayer,
            null,
            (JoltPhysicsSharp.Activation)message.activation
        );
        _body2Owner[bodyId.ID] = connectionid;
        Log.Information($"生成Plane成功:{bodyId}");
    }

    private void OnCmdSpawnBox(in int connectionid, in CmdSpawnBox message)
    {
        Log.Information($"客户端{connectionid}请求生成Box");
        Debug.Assert(float.IsNaN(message.rotation.X) == false);
        Debug.Assert(float.IsNaN(message.rotation.Y) == false);
        Debug.Assert(float.IsNaN(message.rotation.Z) == false);
        Debug.Assert(float.IsNaN(message.rotation.W) == false);


        var bodyId = _app.CreateBox(
            message.halfExtents,
            message.position,
            message.rotation,
            (JoltPhysicsSharp.MotionType)message.motionType,
            (ushort)message.objectLayer,
            (JoltPhysicsSharp.Activation)message.activation
        );
        _body2Owner[bodyId.ID] = connectionid;
        Log.Information($"生成Box成功:{bodyId}");
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
        Log.Information($"JoltUnityDebugger Start {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        // _server.Run(false);
        _server.Run(true);
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
        // _server.socket.TickIncoming();
    }


    public unsafe void AfterUpdate(in JoltApplication.LoopContex ctx)
    {
        // Console.WriteLine($"AfterUpdate {ctx.CurrentFrame}");
        BodyData[] array = ArrayPool<BodyData>.Shared.Rent(_app.bodies.Count);
        var bodies = new ArraySegment<BodyData>(array, 0, _app.bodies.Count);
        WorldData worldData;
        worldData.bodies = bodies;
        worldData.worldId = _app.worldId;
        worldData.gravity = _app.physicsSystem.Gravity;
        worldData.timeStamp = (long)_app.ctx.FrameBeginTimestamp.TotalMicroseconds;
        worldData.frameCount = _app.ctx.CurrentFrame;


        for (var i = 0; i < _app.bodies.Count; i++)
        {
            var id = _app.bodies[i];
            Debug.Assert(_app.physicsSystem.BodyInterface.IsAdded(id));
            var shape = _app.physicsSystem.BodyInterface.GetShape(id);
            ShapeData shapeData = default;
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
                    ShapeData.Create(box, out shapeData);
                    break;
                case ConvexHullShape convexHullShape:
                    break;
                case CylinderShape cylinderShape:
                    break;
                case OffsetCenterOfMassShape offsetCenterOfMassShape:
                    break;
                case SphereShape sphereShape:
                    var sphere = new SphereShapeData(sphereShape.Radius);
                    ShapeData.Create(sphere, out shapeData);
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
                    ShapeData.Create(plane, out shapeData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape));
            }

            Debug.Assert(shapeData.payload is { Array: not null, Count: > 0 });

            if (!_body2Owner.TryGetValue(id, out var ownerId))
            {
                ownerId = ServerId;
            }

            Vector3 position = _app.physicsSystem.BodyInterface.GetPosition(id);
            Quaternion rotation = _app.physicsSystem.BodyInterface.GetRotation(id);


            _app.physicsSystem.BodyLockInterface.LockRead(id, out var @lock);

            bool isSensor = false;
            if (@lock.Succeeded)
            {
                Debug.Assert(@lock.Body != null);
                isSensor = @lock.Body.IsSensor;
            }


            bodies[i] = new BodyData()
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
                shapeData = shapeData
            };


            Debug.Assert(float.IsNaN(bodies[i].position.X) == false);
            Debug.Assert(float.IsNaN(bodies[i].position.Y) == false);
            Debug.Assert(float.IsNaN(bodies[i].position.Z) == false);

            Debug.Assert(float.IsNaN(bodies[i].rotation.X) == false);
            Debug.Assert(float.IsNaN(bodies[i].rotation.Y) == false);
            Debug.Assert(float.IsNaN(bodies[i].rotation.Z) == false);
            Debug.Assert(float.IsNaN(bodies[i].rotation.W) == false);
        }


        _worldSnapshot.PushBack(worldData);
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
using System.Buffers;
using System.Diagnostics;
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
    }

    private void OnCmdSpawnBox(in int connectionid, in CmdSpawnBox message)
    {
        Log.Information($"客户端{connectionid}请求生成Box");
        var bodyId = _app.CreateBox(
            message.halfExtents,
            message.position,
            message.rotation,
            (JoltPhysicsSharp.MotionType)message.motionType,
            (ushort)message.objectLayer,
            (JoltPhysicsSharp.Activation)message.activation
        );
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
        _server.Run();
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

        lock (_app.bodies)
        {
            for (var i = 0; i < _app.bodies.Count; i++)
            {
                var id = _app.bodies[i];
                _app.physicsSystem.BodyLockInterface.LockRead(id, out var @lock);
                if (@lock.Succeeded)
                {
                    var body = @lock.Body;
                    Debug.Assert(body != null);

                    var shape = body.Shape;
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
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(shape));
                    }

                    Debug.Assert(shapeData.payload is { Array: not null, Count: > 0 });

                    bodies[i] = new BodyData()
                    {
                        // TODO Owner ID 根据客户端进行设置
                        ownerId = ServerId,
                        entityId = id.ID,
                        bodyType = (GameCore.Jolt.BodyType)body.BodyType,
                        isActive = body.IsActive,
                        isStatic = body.IsStatic,
                        isKinematic = body.IsKinematic,
                        isDynamic = body.IsDynamic,
                        isSensor = body.IsSensor,
                        objectLayer = body.ObjectLayer,
                        broadPhaseLayer = body.BroadPhaseLayer,
                        allowSleeping = body.AllowSleeping,
                        friction = body.Friction,
                        restitution = body.Restitution,
                        position = body.Position,
                        rotation = body.Rotation,
                        centerOfMass = body.CenterOfMassPosition,
                        linearVelocity = body.GetLinearVelocity(),
                        angularVelocity = body.GetAngularVelocity(),
                        shapeData = shapeData
                    };
                }

                _app.physicsSystem.BodyLockInterface.UnlockRead(@lock);
            }
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
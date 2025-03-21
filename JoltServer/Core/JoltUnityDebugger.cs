using System.Diagnostics;
using System.Runtime.CompilerServices;
using Cysharp.Threading;
using GameCore.Jolt;
using JoltPhysicsSharp;
using Network;
using Network.Server;
using Network.Telepathy;
using UnityToolkit;

namespace JoltServer;

// TODO 将复制物理世界 网络传输部分 在另一个线程执行
public class JoltUnityDebugger : JoltApplication.ISystem
{
    private JoltApplication _app;

    private readonly LogicLooper _networkLooper;

    public int targeFrameRate { get; private set; }

    private WorldData _worldData;
    public NetworkServer _server;
    private int _port;
    public const int MaxMessageSize = 16 * 1024;

    // private CircularBuffer<WorldData> snapshotBuffer;

    public JoltUnityDebugger(int targetFrameRate, int port, int bufferSize = 10)
    {
        // snapshotBuffer = new CircularBuffer<WorldData>(bufferSize);
        if (port > ushort.MaxValue)
        {
            throw new ArgumentException("Port must be less than or equal to 65535");
        }

        _server = new NetworkServer(new TelepathyServerSocket((ushort)port), (ushort)targetFrameRate, true);
        _port = port;
        targeFrameRate = targetFrameRate;
        _networkLooper = new LogicLooper(targetFrameRate);
    }

    public void OnAdded(JoltApplication app)
    {
        _app = app;
    }

    public void OnRemoved()
    {
    }

    private long _lastWorldTimeStamp;

    public void BeforeRun()
    {
        Console.WriteLine($"JoltUnityDebugger Start {_port}");
        _server.Run();
        _networkLooper.RegisterActionAsync((in LogicLooperActionContext ctx) =>
        {
            Console.WriteLine("JoltUnityDebugger Update");
            // lock (snapshotBuffer)
            // {
            //     var worldData = snapshotBuffer.Back(); // TODO Using ref to void copy
            //     if (worldData.timeStamp == _lastWorldTimeStamp)
            //     {
            //         return true;
            //     }
            //
            ref var worldData = ref _data;
            _server.SendToAll(worldData);
            Console.WriteLine($"Send WorldData {worldData.worldId} {worldData.frameCount} {worldData.timeStamp}");
            _lastWorldTimeStamp = worldData.timeStamp;
            // }

            return true;
        });
    }


    public void BeforeUpdate(in JoltApplication.LoopContex ctx)
    {
    }

    public const int ServerId = 0;

    public unsafe void AfterUpdate(in JoltApplication.LoopContex ctx)
    {
        // Console.WriteLine($"AfterUpdate {ctx.CurrentFrame}");
        BodyData[] bodies = new BodyData[_app.bodies.Count];
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
                    IShapeData shapeData = null!;

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
                            shapeData = new BoxShapeData(boxShape.HalfExtent);
                            break;
                        case ConvexHullShape convexHullShape:
                            break;
                        case CylinderShape cylinderShape:
                            break;
                        case OffsetCenterOfMassShape offsetCenterOfMassShape:
                            break;
                        case SphereShape sphereShape:
                            shapeData = new SphereShapeData(sphereShape.Radius);
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

        // Console.WriteLine($"Push WorldData {worldData.worldId} {worldData.frameCount} {worldData.timeStamp}");
        // snapshotBuffer.PushBack(worldData);
        _data = worldData;
    }

    private WorldData _data;

    public void AfterRun()
    {
        _server.Stop();
        _networkLooper.ShutdownAsync(TimeSpan.Zero).Wait();
    }


    public void Dispose()
    {
    }
}
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
using ShapeType = JoltPhysicsSharp.ShapeType;

namespace JoltServer;

// TODO 将复制物理世界 网络传输部分 在另一个线程执行
public partial class JoltServer : JoltApplication.ISystem
{

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


    public void PackBodyData(in BodyID id, out BodyData data) => _app.physicsWorld.QueryBody(id, out data);

    public unsafe void AfterUpdate(in JoltApplication.LoopContex ctx)
    {
        // Console.WriteLine($"AfterUpdate {ctx.CurrentFrame}");
        BodyData[] array = ArrayPool<BodyData>.Shared.Rent(_app.physicsWorld.bodies.Count);
        var bodies = new ArraySegment<BodyData>(array, 0, _app.physicsWorld.bodies.Count);
        WorldData worldData;
        worldData.bodies = bodies;
        // worldData.worldId = _app.worldId;
        worldData.gravity = _app.physicsWorld.physicsSystem.Gravity;
        worldData.timeStamp = (long)_app.ctx.FrameBeginTimestamp.TotalMicroseconds;
        worldData.frameCount = _app.ctx.CurrentFrame;


        for (var i = 0; i < _app.physicsWorld.bodies.Count; i++)
        {
            var id = _app.physicsWorld.bodies[i];
            Debug.Assert(_app.physicsWorld.physicsSystem.BodyInterface.IsAdded(id));
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
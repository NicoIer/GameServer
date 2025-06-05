using System.Buffers;
using System.Diagnostics;
using Cysharp.Threading;
using GameCore.Jolt;
using JoltPhysicsSharp;
using Network.Server;
using Serilog;
using UnityToolkit;

namespace JoltServer;

// TODO 将复制物理世界 网络传输部分 在另一个线程执行
public partial class JoltServer : IJoltSystem<JoltApplication,JoltApplication.LoopContex>
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

    public long currentFrame { get; private set; }

    // private FrameStep _frameStep;


    public delegate void ContextDelegate(in JoltApplication.LoopContex ctx);

    private JoltConfig _config;

    public JoltServer(int targetFrameRate, int port, int bufferSize, JoltConfig cfg)
    {
        _config = cfg;
        // _frameStep = new FrameStep(bufferSize);
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

        if (cfg.lockStep)
        {
            HandleLockStep();
        }
    }


    public void OnAdded(JoltApplication app,IPhysicsWorld world)
    {
        _app = app;
    }

    public void OnRemoved()
    {
    }


    public void BeforeRun()
    {
        HandleRpc();
        Log.Information($"JoltServer Start {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        _server.Run(false);
        if (_config.lockStep)
        {
            StartLockStep();
        }

        // _server.Run(true);
        _networkLooper.RegisterActionAsync((in LogicLooperActionContext ctx) =>
        {
            if (_worldSnapshot.IsEmpty) return true;
            ref var worldData = ref _worldSnapshot.Back(); //.buffer[_worldSnapshot.backIndex];
            if (worldData.frameCount != _lastSendWorldFrame && _server.ConnectionCount > 0)
            {
                _server.SendToAll(worldData);
                _lastSendWorldFrame = worldData.frameCount;
            }

            return true;
        });
    }


    public void BeforeUpdate(in JoltApplication.LoopContex ctx)
    {
        if (_config.lockStep)
        {
            LockStep(ctx);
        }

        _server.socket.TickIncoming();
    }


    public unsafe void AfterUpdate(in JoltApplication.LoopContex ctx)
    {
        // Console.WriteLine($"AfterUpdate {ctx.CurrentFrame}");
        BodyData[] array = ArrayPool<BodyData>.Shared.Rent(_app.physicsWorld.bodies.Count);
        var bodies = new ArraySegment<BodyData>(array, 0, _app.physicsWorld.bodies.Count);

        WorldData worldData = new WorldData
        {
            bodies = bodies,
            timeStamp = (long)_app.ctx.FrameBeginTimestamp.TotalMicroseconds,
            frameCount = _app.ctx.CurrentFrame
        };
        _app.physicsWorld.Serialize(ref worldData);


        _worldSnapshot.PushBack(worldData);

        currentFrame = ctx.CurrentFrame;

        _server.socket.TickOutgoing();
    }


    public void AfterRun()
    {
        _server.Stop();
        if (_config.lockStep)
        {
            StopLockStep();
        }

        _networkLooper.ShutdownAsync(TimeSpan.Zero).Wait();
    }


    public void Dispose()
    {
    }


    public ref WorldData QueryHistoryWorld(int delta)
    {
        Debug.Assert(currentFrame == _worldSnapshot.backValue.frameCount);
        long targetFrame = currentFrame + delta;

        for (int i = 0; i < _worldSnapshot.Size; i++)
        {
            ref var snapshot = ref _worldSnapshot.buffer[i];
            if (snapshot.frameCount == targetFrame)
            {
                return ref snapshot;
            }
        }

        throw new ArgumentException($"WorldData {targetFrame} not found delta={delta} out of range");
    }
}
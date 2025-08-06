using Cysharp.Threading;
using Game001;
using GameCore.Physics;
using JoltPhysicsSharp;
using JoltServer;
using Network.Server;
using Serilog;
using UnityToolkit;
using WorldData = Game001.WorldData;

namespace Soccer;

public partial class SoccerGameServer :
    IJoltSystem<JoltApplication,
        JoltApplication.LoopContex,
        JoltPhysicsWorld>
{
    public int FrameRate { get; private set; }
    private JoltApplication _app;
    private readonly LogicLooper _networkLooper;

    public PhysicsSystem physics => _app.physicsWorld.physicsSystem;
    // private readonly CircularBuffer<WorldData> _worldSnapshot;

    private readonly NetworkServer _server;
    private int port;

    public SoccerGameServer(int frameRate, int port)
    {
        if (port > ushort.MaxValue)
        {
            throw new ArgumentException("Port must be less than or equal to 65535");
        }

        this.port = port;
        FrameRate = frameRate;
        _networkLooper = new LogicLooper(frameRate);
        _server = new NetworkServer(new TelepathyServerSocket((ushort)port), (ushort)frameRate, true);
    }

    public void OnAdded(JoltApplication app, JoltPhysicsWorld world)
    {
        _app = app;
    }

    public void OnRemoved()
    {
    }

    public void BeforePhysicsStart()
    {
        HandlePhysicsInit();
        Log.Information(
            "SoccerGameServer Start at port {Port} with frame rate {FrameRate:yyyy-MM-dd HH:mm:ss} , {Time}",
            port, FrameRate, DateTime.Now);
        _server.Run(false);
        _networkLooper.RegisterActionAsync(OnNetworkTick);
    }

    private bool OnNetworkTick(in LogicLooperActionContext ctx)
    {
        return true;
    }

    public void BeforePhysicsUpdate(in JoltApplication.LoopContex ctx)
    {
        _server.socket.TickIncoming();
    }

    public void AfterPhysicsUpdate(in JoltApplication.LoopContex ctx)
    {
        // 收集物理世界信息
        PlayerData redPlayer = new PlayerData
        {
            position = redPlayer1.Position,
            rotation = redPlayer1.Rotation,
            linearVelocity = redPlayer1.GetLinearVelocity(),
            angularVelocity = redPlayer1.GetAngularVelocity()
        };

        PlayerData bluePlayer = new PlayerData
        {
            position = bluePlayer1.Position,
            rotation = bluePlayer1.Rotation,
            linearVelocity = bluePlayer1.GetLinearVelocity(),
            angularVelocity = bluePlayer1.GetAngularVelocity()
        };

        SoccerData soccer = new SoccerData
        {
            position = soccerBall.Position,
            rotation = soccerBall.Rotation,
            linearVelocity = soccerBall.GetLinearVelocity(),
            angularVelocity = soccerBall.GetAngularVelocity()
        };

        WorldData worldData = new WorldData
        {
            redPlayer = redPlayer,
            bluePlayer = bluePlayer,
            soccer = soccer
        };

        _server.SendToAll(worldData);
        Log.Information("Send WorldData {worldData}", worldData);
        _server.socket.TickOutgoing();
    }

    public void AfterPhysicsStop()
    {
        _server.Stop();
        _networkLooper.ShutdownAsync(TimeSpan.Zero).Wait();
    }

    public void Dispose()
    {
    }
}
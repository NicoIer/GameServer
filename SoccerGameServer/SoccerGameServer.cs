using System.Numerics;
using Cysharp.Threading;
using GameCore.Physics;
using JoltPhysicsSharp;
using JoltServer;
using Network.Server;
using Serilog;

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
        HandleCmd();
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
        if (ctx.CurrentFrame % 120 == 0)
        {
            Log.Information("SoccerGameServer CurrentFrame{CurrentFrame}", ctx.CurrentFrame);
        }

        _server.socket.TickIncoming();

        if (redWin)
        {
            ResetGameWorld();
        }
        else if (blueWin)
        {
            ResetGameWorld();
        }

        redWin = false;
        blueWin = false;

        var bodyInterface = physics.BodyInterface;
        if (redPlayerInput.Length() > 0.01)
        {
            // Log.Information("Red Input {redPlayerInput}", redPlayerInput);
            Vector3 redMoveVector = new Vector3(redPlayerInput.X, 0, redPlayerInput.Y);
            // redPlayer1.AddImpulse(redMoveVector * 30);
            // redPlayer1.AddForce(redMoveVector * 30);
            // bodyInterface.AddLinearVelocity(redPlayer1.ID, redMoveVector);
            bodyInterface.AddForce(redPlayer1.ID, redMoveVector * 30 * 3000);
            redPlayerInput = Vector2.Zero;
        }

        if (bluePlayerInput.Length() > 0.01)
        {
            // Log.Information("Blue Input {bluePlayerInput}", bluePlayerInput);
            Vector3 blueMoveVector = new Vector3(bluePlayerInput.X, 0, bluePlayerInput.Y);
            // bluePlayer1.AddForce(blueMoveVector * 30);
            // bluePlayer1.AddImpulse(blueMoveVector * 30);
            // bodyInterface.AddLinearVelocity(bluePlayer1.ID, blueMoveVector);
            bodyInterface.AddForce(bluePlayer1.ID, blueMoveVector * 30 * 3000);
            bluePlayerInput = Vector2.Zero;
        }
    }

    public void AfterPhysicsUpdate(in JoltApplication.LoopContex ctx)
    {
        CheckSoccer();
        BroadcastWorldData();
        _server.socket.TickOutgoing();
    }

    private void CheckSoccer()
    {
    }

    private void BroadcastWorldData()
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
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

    public int redScore;
    public int blueScore;

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

        var soccerPos = soccerBall.Position;


        if (redWin)
        {
            if (soccerPos.Y <= 3)
            {
                _server.SendToAll(new RpcPlayerGoal(IdentifierEnum.RedPlayer));
                ++redScore;
            }

            ResetGameWorld();
        }
        else if (blueWin)
        {
            if (soccerPos.Y <= 3)
            {
                _server.SendToAll(new RpcPlayerGoal(IdentifierEnum.BluePlayer));
                ++blueScore;
            }

            ResetGameWorld();
        }

        // 如果足球离开了场地范围
        if (Math.Abs(soccerPos.X) > 14 || Math.Abs(soccerPos.Z) > 6)
        {
            Log.Information("足球不知道跑哪去了");
            ResetGameWorld();
        }


        redWin = false;
        blueWin = false;

        var bodyInterface = physics.BodyInterface;
        if (redPlayerInput.moveInput.Length() > 0.01)
        {
            // Log.Information("Red Input {redPlayerInput}", redPlayerInput);
            Vector3 redMoveVector = new Vector3(redPlayerInput.moveInput.X, 0, redPlayerInput.moveInput.Y);
            // redPlayer1.AddImpulse(redMoveVector * 30);
            // redPlayer1.AddForce(redMoveVector * 30);
            // bodyInterface.AddLinearVelocity(redPlayer1.ID, redMoveVector);
            bodyInterface.AddForce(redPlayer1.ID, redMoveVector * 30 * 2500);
            redPlayerInput.moveInput = Vector2.Zero;
        }

        if (bluePlayerInput.moveInput.Length() > 0.01)
        {
            // Log.Information("Blue Input {bluePlayerInput}", bluePlayerInput);
            Vector3 blueMoveVector = new Vector3(bluePlayerInput.moveInput.X, 0, bluePlayerInput.moveInput.Y);
            // bluePlayer1.AddForce(blueMoveVector * 30);
            // bluePlayer1.AddImpulse(blueMoveVector * 30);
            // bodyInterface.AddLinearVelocity(bluePlayer1.ID, blueMoveVector);
            bodyInterface.AddForce(bluePlayer1.ID, blueMoveVector * 30 * 2500);
            bluePlayerInput.moveInput = Vector2.Zero;
        }

        redPlayerContactSoccer |= Vector3.Distance(redPlayer1.Position, soccerBall.Position) < 0.6f;
        bluePlayerContactSoccer |= Vector3.Distance(bluePlayer1.Position, soccerBall.Position) < 0.6f;

        if (redPlayerContactSoccer && bluePlayerContactSoccer && redPlayerInput.kickPressed > 0 &&
            bluePlayerInput.kickPressed > 0)
        {
            if (Random.Shared.NextDouble() < 0.5)
            {
                KickSoccer(redPlayer1, redPlayerInput.kickPressed);
            }
            else
            {
                KickSoccer(bluePlayer1, bluePlayerInput.kickPressed);
            }
        }
        else
        {
            if (redPlayerContactSoccer && redPlayerInput.kickPressed > 0)
            {
                KickSoccer(redPlayer1, redPlayerInput.kickPressed);
            }

            if (bluePlayerContactSoccer && bluePlayerInput.kickPressed > 0)
            {
                KickSoccer(bluePlayer1, bluePlayerInput.kickPressed);
            }
        }

        redPlayerInput.kickPressed = 0;
        // float.Lerp(redPlayerInput.kickPressed, 0,
        // ctx.ElapsedTimeFromPreviousFrame.Milliseconds / 1000f);
        bluePlayerInput.kickPressed = 0;
        // float.Lerp(bluePlayerInput.kickPressed, 0,
        // ctx.ElapsedTimeFromPreviousFrame.Milliseconds / 1000f);
    }

    private void KickSoccer(Body player, float rate)
    {
        Log.Information("Kick Soccer {who}", player == redPlayer1 ? "Red" : "Blue");
        Vector3 direction = player.GetLinearVelocity();
        direction.Y = 0;
        direction = Vector3.Normalize(direction);
        // 往(0,1,0) 旋转45度
        direction = Vector3.Normalize((direction + new Vector3(0, 1, 0)) / 2);
        soccerBall.AddImpulse(direction * 30 * 100 * rate);
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
            redScore = redScore,
            blueScore = blueScore,
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
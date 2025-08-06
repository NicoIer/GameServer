using System.Numerics;
using GameCore.Physics;
using JoltPhysicsSharp;
using Activation = JoltPhysicsSharp.Activation;
using AllowedDOFs = JoltPhysicsSharp.AllowedDOFs;
using MotionType = JoltPhysicsSharp.MotionType;

namespace Soccer;

public partial class SoccerGameServer
{
    public Body soccerBall;
    public Body ground;

    public Body blueGoal;
    public Body redGoal;

    public Body bluePlayer1;
    public Body redPlayer1;

    public void HandlePhysicsInit()
    {
        _app.physicsWorld.physicsSystem.OnContactAdded += OnContactAdded;
        _app.physicsWorld.physicsSystem.OnContactRemoved += OnContactRemoved;
        _app.physicsWorld.physicsSystem.OnContactPersisted += OnContactPersisted;
        // 中心(0,-0.5,0) 20*1*10 的静态盒子 用于当作地面
        BoxShapeSettings boxShapeSettings =
            new BoxShapeSettings(new Vector3(10 + 3, 0.5f, 5), Foundation.DefaultConvexRadius);
        BodyCreationSettings boxBodySettings = new BodyCreationSettings(boxShapeSettings, new Vector3(0, -0.5f, 0),
            Quaternion.Identity, MotionType.Static, new ObjectLayer((uint)ObjectLayers.NonMoving));
        ground = physics.BodyInterface.CreateBody(boxBodySettings);


        // 球体(0,0.5,0) 半径0.5 的动态球体 Mass 3 Linear Damping 0 Angular Daming 0.05 动摩擦力0.6 静摩擦力0.6 Bounciness0.9 用于当作足球
        SphereShapeSettings sphereShapeSettings = new SphereShapeSettings(0.5f);
        BodyCreationSettings sphereBodySettings =
            new BodyCreationSettings(sphereShapeSettings, new Vector3(0, 0.5f, 0), Quaternion.Identity,
                MotionType.Dynamic, new ObjectLayer((uint)ObjectLayers.Moving));
        sphereBodySettings.MassPropertiesOverride = new MassProperties()
        {
            Mass = 3,
        };
        sphereBodySettings.LinearDamping = 0;
        sphereBodySettings.AngularDamping = 0.05f;
        sphereBodySettings.Friction = 0.6f;
        sphereBodySettings.Restitution = 0.9f; // Bounciness
        soccerBall = physics.BodyInterface.CreateBody(sphereBodySettings);

        // 两个球门 （-13.5,1,0) 和 (13.5,1,0) 大小(1,2,10) 的静态盒子
        BoxShapeSettings goalShapeSettings =
            new BoxShapeSettings(new Vector3(0.5f, 1, 5), Foundation.DefaultConvexRadius);
        BodyCreationSettings goalBodySettings1 = new BodyCreationSettings(goalShapeSettings, new Vector3(-13.5f, 1, 0),
            Quaternion.Identity, MotionType.Static, new ObjectLayer((uint)ObjectLayers.NonMoving));
        BodyCreationSettings goalBodySettings2 = new BodyCreationSettings(goalShapeSettings, new Vector3(13.5f, 1, 0),
            Quaternion.Identity, MotionType.Static, new ObjectLayer((uint)ObjectLayers.NonMoving));
        blueGoal = physics.BodyInterface.CreateBody(goalBodySettings1);
        redGoal = physics.BodyInterface.CreateBody(goalBodySettings2);

        // 两个球员 （-3,1,0) 和 (3,1,0) 大小(1,1,1) 的动态盒子 不可旋转 Kinematic
        // (0,90,0) & (0,-90,0) 的旋转
        Quaternion bluePlayerRotation = Quaternion.CreateFromYawPitchRoll(MathF.PI / 2, 0, 0);
        Quaternion redPlayerRotation = Quaternion.CreateFromYawPitchRoll(-MathF.PI / 2, 0, 0);
        BoxShapeSettings playerShapeSettings =
            new BoxShapeSettings(new Vector3(0.5f, 0.5f, 0.5f), Foundation.DefaultConvexRadius);
        BodyCreationSettings redPlayerBodySettings1 = new BodyCreationSettings(playerShapeSettings,
            new Vector3(-3, 1, 0),
            bluePlayerRotation, MotionType.Dynamic, new ObjectLayer((uint)ObjectLayers.Moving));
        BodyCreationSettings bluePlayerBodySettings2 = new BodyCreationSettings(playerShapeSettings,
            new Vector3(3, 1, 0),
            redPlayerRotation, MotionType.Dynamic, new ObjectLayer((uint)ObjectLayers.Moving));

        SetupPlayer(redPlayerBodySettings1);
        SetupPlayer(bluePlayerBodySettings2);

        bluePlayer1 = physics.BodyInterface.CreateBody(redPlayerBodySettings1);
        redPlayer1 = physics.BodyInterface.CreateBody(bluePlayerBodySettings2);

        // 添加到世界
        physics.BodyInterface.AddBody(soccerBall, Activation.Activate);
        physics.BodyInterface.AddBody(ground, Activation.Activate);
        physics.BodyInterface.AddBody(blueGoal, Activation.Activate);
        physics.BodyInterface.AddBody(redGoal, Activation.Activate);
        physics.BodyInterface.AddBody(bluePlayer1, Activation.Activate);
        physics.BodyInterface.AddBody(redPlayer1, Activation.Activate);
    }

    private void SetupPlayer(BodyCreationSettings settings)
    {
        settings.AllowedDOFs = AllowedDOFs.TranslationX | AllowedDOFs.TranslationZ; // 只允许X Z平移
        settings.MassPropertiesOverride = new MassProperties()
        {
            Mass = 30
        };
        settings.LinearDamping = 3f;
        settings.AngularDamping = 0.05f;
    }

    private void OnContactPersisted(PhysicsSystem system, in Body body1, in Body body2, in ContactManifold manifold,
        in ContactSettings settings)
    {
    }

    private void OnContactRemoved(PhysicsSystem system, ref SubShapeIDPair subShapePair)
    {
    }

    private void OnContactAdded(PhysicsSystem system, in Body body1, in Body body2, in ContactManifold manifold,
        in ContactSettings settings)
    {
    }
}
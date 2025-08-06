using System.Numerics;
using GameCore.Physics;
using JoltPhysicsSharp;
using Activation = JoltPhysicsSharp.Activation;
using MotionType = JoltPhysicsSharp.MotionType;

namespace Soccer;

public partial class SoccerGameServer
{
    public Body soccer;
    public Body ground;
    public void HandlePhysicsInit()
    {
        // 中心(0,-0.5,0) 20*1*10 的静态盒子 用于当作地面
        BoxShapeSettings boxShapeSettings =
            new BoxShapeSettings(new Vector3(10, 0.5f, 5), Foundation.DefaultConvexRadius);
        BodyCreationSettings boxBodySettings = new BodyCreationSettings(boxShapeSettings, new Vector3(0, -0.5f, 0),
            Quaternion.Identity, MotionType.Static, new ObjectLayer((uint)ObjectLayers.NonMoving));
        physics.BodyInterface.CreateAndAddBody(boxBodySettings, Activation.Activate);

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
        
    }
}
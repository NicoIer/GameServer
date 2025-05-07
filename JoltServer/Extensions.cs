using GameCore.Jolt;
using JoltPhysicsSharp;

namespace JoltServer;

public static class Extensions
{
    public static void CreateWorld(this JoltApplication application, WorldData worldData)
    {
        throw new NotImplementedException();
    }

    // public static void Transform(this BodyID id,in int ownerId, in BodyInterface bodyInterface, out BodyData data)
    // {
    //     data = new BodyData()
    //     {
    //         ownerId = ownerId,
    //         entityId = id.ID,
    //         bodyType = (GameCore.Jolt.BodyType)bodyInterface.GetBodyType(id),
    //         isActive = bodyInterface.IsActive(id),
    //         motionType = (GameCore.Jolt.MotionType)bodyInterface.GetMotionType(id),
    //         isSensor = isSensor,
    //         objectLayer = bodyInterface.GetObjectLayer(id),
    //         friction = bodyInterface.GetFriction(id),
    //         restitution = bodyInterface.GetRestitution(id),
    //         position = position,
    //         rotation = rotation,
    //         centerOfMass = bodyInterface.GetCenterOfMassPosition(id),
    //         linearVelocity = bodyInterface.GetLinearVelocity(id),
    //         angularVelocity = bodyInterface.GetAngularVelocity(id),
    //         shapeData = shapeData
    //     };
    // }

    // public static void Transform(this Body body, int ownerId, out BodyData data)
    // {
    //     data = new BodyData()
    //     {
    //         ownerId = ownerId,
    //         entityId = body.ID,
    //         bodyType = (GameCore.Jolt.Shared.BodyType)body.BodyType,
    //         isActive = body.IsActive,
    //         motionType = (GameCore.Jolt.Shared.MotionType)body.MotionType,
    //         isSensor = body.IsSensor,
    //         objectLayer = body.ObjectLayer,
    //         friction = body.Friction,
    //         restitution = body.Restitution,
    //         position = body.Position,
    //         rotation = body.Rotation,
    //         centerOfMass = body.CenterOfMassPosition,
    //         linearVelocity = body.GetLinearVelocity(),
    //         angularVelocity = body.GetAngularVelocity(),
    //         shapeData = shapeData
    //     };
    // }
}
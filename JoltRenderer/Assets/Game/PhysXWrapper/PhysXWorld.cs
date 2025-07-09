using GameCore.Physics;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Game.PhysXWrapper
{
    public class PhysXWorld : MonoBehaviour , IPhysicsWorld
    {
        public byte worldId { get; }
        public PhysicsUpdateError Simulate(in float deltaTime, in int collisionSteps)
        {
            throw new System.NotImplementedException();
        }

        public uint CreateAndAdd(IShapeData shapeData, in Vector3 position, in Quaternion rotation, MotionType motionType,
            ObjectLayers layers, Activation activation)
        {
            throw new System.NotImplementedException();
        }

        public bool Exist(in uint id)
        {
            throw new System.NotImplementedException();
        }

        public bool QueryBody(in uint id, out BodyData bodyData)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetPosition(in uint id)
        {
            throw new System.NotImplementedException();
        }

        public Quaternion GetRotation(in uint id)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateBody(in uint id, in BodyData bodyData)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref WorldData worldData)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(in WorldData worldData)
        {
            throw new System.NotImplementedException();
        }

        public void Activate(in uint id)
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate(in uint id)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAndDestroy(in uint id)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
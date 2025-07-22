using System;
using System.Collections.Generic;
using System.Threading;
using GameCore.Physics;
using Jolt;
using Unity.Mathematics;
using UnityEngine;
using UnityToolkit;
using Activation = GameCore.Physics.Activation;
using MotionType = GameCore.Physics.MotionType;
using PhysicsUpdateError = GameCore.Physics.PhysicsUpdateError;
using Quaternion = System.Numerics.Quaternion;
using ShapeSubType = Jolt.ShapeSubType;
using Vector3 = System.Numerics.Vector3;
using JoltApi = Jolt.Bindings;

namespace JoltWrapper
{
    public class JoltPhysicsWorld : IPhysicsWorld
    {
        public byte worldId { get; private set; }
        public readonly PhysicsSystem physicsSystem;
        private readonly JobSystem _nativeJobSystem = JobSystem.Create();
        public float physicsTime { get; private set; }

        private readonly List<uint> _bodies = new();
        public IReadOnlyList<uint> bodies => _bodies;

        public JoltPhysicsWorld()
        {
            if (!Jolt.Jolt.Initialized)
            {
                throw new Exception(
                    "Jolt is not initialized. Please call Jolt.Initialize() before creating a physics world.");
            }

            Interlocked.Increment(ref IPhysicsWorld.worldIdCounter);
            if (IPhysicsWorld.worldIdCounter > byte.MaxValue) throw new Exception("WorldId overflow");
            worldId = (byte)IPhysicsWorld.worldIdCounter;

            #region Setup

            ObjectLayerPairFilterTable objectLayerPairFilter = new(2);
            objectLayerPairFilter.EnableCollision((uint)ObjectLayers.NonMoving, (uint)ObjectLayers.Moving);
            objectLayerPairFilter.EnableCollision((uint)ObjectLayers.Moving, (uint)ObjectLayers.Moving);

            // We use a 1-to-1 mapping between object layers and broadphase layers
            BroadPhaseLayerInterfaceTable broadPhaseLayerInterface = new(2, 2);
            broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((uint)ObjectLayers.NonMoving,
                (byte)BroadPhaseLayers.NonMoving);
            broadPhaseLayerInterface.MapObjectToBroadPhaseLayer((uint)ObjectLayers.Moving,
                (byte)BroadPhaseLayers.Moving);

            ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter =
                new(broadPhaseLayerInterface, 2, objectLayerPairFilter, 2);

            var settings = new PhysicsSystemSettings
            {
                MaxBodies = IPhysicsWorld.MaxBodies,
                MaxBodyPairs = IPhysicsWorld.MaxBodyPairs,
                MaxContactConstraints = IPhysicsWorld.MaxContactConstraints,
                ObjectLayerPairFilter = objectLayerPairFilter,
                BroadPhaseLayerInterface = broadPhaseLayerInterface,
                ObjectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter,
            };

            #endregion

            physicsSystem = new PhysicsSystem(settings);
            
        }


        public PhysicsUpdateError Simulate(in float deltaTime, in int collisionSteps)
        {
            if (physicsSystem.Update(deltaTime, collisionSteps, _nativeJobSystem, out var error))
            {
                physicsTime += deltaTime;
            }
            else
            {
                Debug.LogError(error);
            }

            return (PhysicsUpdateError)error;
        }

        public uint CreateAndAdd(IShapeData shapeData, in Vector3 position, in Quaternion rotation,
            MotionType motionType,
            ObjectLayers layers, Activation activation)
        {
            Shape? shape;
            switch (shapeData)
            {
                case BoxShapeData boxShapeData:
                    float3 halfExtents = new(
                        boxShapeData.halfExtents.X,
                        boxShapeData.halfExtents.Y,
                        boxShapeData.halfExtents.Z
                    );
                    shape = new BoxShape(halfExtents, boxShapeData.convexRadius);
                    break;
                case SphereShapeData sphereShapeData:
                    shape = new SphereShape(sphereShapeData.radius);
                    break;
                case CapsuleShapeData capsuleShapeData:
                    shape = new CapsuleShape(
                        capsuleShapeData.halfHeight,
                        capsuleShapeData.radius
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shapeData));
            }

            if (shape == null)
            {
                throw new ArgumentException($"cannot create shape settings from shape[{shapeData}] ");
            }

            quaternion quaternion = new(rotation.X, rotation.Y, rotation.Z, rotation.W);
            var settings = new BodyCreationSettings(
                shape.Value,
                position,
                quaternion,
                (Jolt.MotionType)motionType,
                (uint)layers
            );
            var bodyInterface = physicsSystem.BodyInterface;

            var body = bodyInterface.CreateAndAddBody(settings, (Jolt.Activation)activation);

            // --------------------- //
            _bodies.Add(body.ID);
            // --------------------- //

            return body.ID;
        }

        public bool IsAdded(in uint id)
        {
            return physicsSystem.BodyInterface.IsAdded(new BodyID(id));
        }

        public bool QueryBody(in uint id, out BodyData bodyData)
        {
            var bodyInterface = physicsSystem.BodyInterface;
            var bodyId = new BodyID(id);
            var shape = bodyInterface.GetShape(bodyId);
            ShapeDataPacket? shapeDataPacket = null;
            if (PackShapeData(shape, out var packet) == false)
            {
                shapeDataPacket = packet;
            }

            var position = bodyInterface.GetPosition(bodyId);
            var quaternion = bodyInterface.GetRotation(bodyId).value;
            var rotation = new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

            var linearVelocity = bodyInterface.GetLinearVelocity(bodyId);
            var angularVelocity = bodyInterface.GetAngularVelocity(bodyId);

            bodyData = new BodyData()
            {
                id = id,
                bodyType = (GameCore.Physics.BodyType)bodyInterface.GetBodyType(bodyId),
                isActive = bodyInterface.IsActive(bodyId),
                motionType = (GameCore.Physics.MotionType)bodyInterface.GetMotionType(bodyId),
                // isSensor = isSensor,
                objectLayer = bodyInterface.GetObjectLayer(bodyId),
                friction = bodyInterface.GetFriction(bodyId),
                restitution = bodyInterface.GetRestitution(bodyId),
                position = position,
                rotation = rotation,
                centerOfMass = bodyInterface.GetCenterOfMassPosition(bodyId),
                linearVelocity = new Vector3(linearVelocity.x, linearVelocity.y, linearVelocity.z),
                angularVelocity = new Vector3(angularVelocity.x, angularVelocity.y, angularVelocity.z),
                shapeDataPacket = shapeDataPacket
            };


            if (shapeDataPacket == null)
            {
                ToolkitLog.Info($"got a null shape packet,id:{id},threadId:{Thread.CurrentThread.ManagedThreadId}");
            }

            return true;
        }

        private static bool PackShapeData(Shape shape, out ShapeDataPacket packet)
        {
            packet = default;
            switch (shape.subType)
            {
                case ShapeSubType.Sphere:
                    var radius = JoltApi.JPH_SphereShape_GetRadius(shape.Handle.Reinterpret<JPH_SphereShape>());
                    ShapeDataPacket.Create(new SphereShapeData(radius), out packet);
                    break;
                case ShapeSubType.Box:
                    var boxShape = shape.Handle.Reinterpret<JPH_BoxShape>();
                    var f3 = JoltApi.JPH_BoxShape_GetHalfExtent(boxShape);
                    var convexRadius = JoltApi.JPH_BoxShape_GetConvexRadius(boxShape);
                    var value = new Vector3(f3.x, f3.y, f3.z);
                    ShapeDataPacket.Create(new BoxShapeData(value,convexRadius), out packet);
                    break;
                case ShapeSubType.Triangle:
                    break;
                case ShapeSubType.Capsule:
                    break;
                case ShapeSubType.TaperedCapsule:
                    break;
                case ShapeSubType.Cylinder:
                    break;
                case ShapeSubType.ConvexHull:
                    break;
                case ShapeSubType.StaticCompound:
                    break;
                case ShapeSubType.MutableCompound:
                    break;
                case ShapeSubType.RotatedTranslated:
                    break;
                case ShapeSubType.Scaled:
                    break;
                case ShapeSubType.OffsetCenterOfMass:
                    break;
                case ShapeSubType.Mesh:
                    break;
                case ShapeSubType.HeightField:
                    break;
                case ShapeSubType.SoftBody:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        public Vector3 GetPosition(in uint id)
        {
            var position = physicsSystem.BodyInterface.GetPosition(id);
            return new Vector3(position.x, position.y, position.z);
        }

        public Quaternion GetRotation(in uint id)
        {
            var rotation = physicsSystem.BodyInterface.GetRotation(id);
            return new Quaternion(rotation.value.x, rotation.value.y, rotation.value.z, rotation.value.w);
        }

        /// <summary>
        /// 序列化物理世界 客户端没有必要实现这个方法
        /// </summary>
        /// <param name="worldData"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Serialize(ref WorldData worldData)
        {
            throw new NotImplementedException("Deserialization is not implemented yet.");
        }

        /// <summary>
        /// 反序列化物理世界 客户端没有必要实现这个方法
        /// </summary>
        /// <param name="worldData"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Deserialize(in WorldData worldData)
        {
            throw new NotImplementedException("Deserialization is not implemented yet.");
        }

        public void Activate(in uint id)
        {
            physicsSystem.BodyInterface.ActivateBody(id);
        }

        public void Deactivate(in uint id)
        {
            physicsSystem.BodyInterface.DeactivateBody(id);
        }

        public void RemoveAndDestroy(in uint id)
        {
            Debug.Assert(IsAdded(id), $"Body with id {id} does not exist in the physics world.");
            var bodyInterface = physicsSystem.BodyInterface;
            var bodyId = new BodyID(id);
            bodyInterface.RemoveAndDestroyBody(bodyId);
            _bodies.Remove(id);
        }

        public void Dispose()
        {
            physicsSystem.Destroy();
        }
    }
}
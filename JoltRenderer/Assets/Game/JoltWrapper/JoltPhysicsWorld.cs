using System;
using System.Collections.Generic;
using System.Threading;
using GameCore.Jolt;
using Jolt;
using Unity.Mathematics;
using UnityEngine;
using Activation = GameCore.Jolt.Activation;
using MotionType = GameCore.Jolt.MotionType;
using PhysicsUpdateError = GameCore.Jolt.PhysicsUpdateError;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace JoltWrapper
{
    public class JoltPhysicsWorld : IPhysicsWorld
    {
        public byte worldId { get; private set; }
        public readonly PhysicsSystem physicsSystem;
        private readonly JobSystem _nativeJobSystem = new();
        public float physicsTime { get; private set; }

        private readonly HashSet<uint> _bodiesSet = new();
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

            ObjectLayerPairFilterTable objectLayerPairFilter = new(2);
            objectLayerPairFilter.EnableCollision((uint)ObjectLayers.NonMoving, (uint)ObjectLayers.Moving);
            objectLayerPairFilter.EnableCollision((uint)ObjectLayers.Moving, (uint)ObjectLayers.Moving);

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

            physicsSystem = new PhysicsSystem(settings);


            // InitializeScene();

            physicsSystem.OptimizeBroadPhase();
        }


        public PhysicsUpdateError Simulate(in float deltaTime, in int collisionSteps)
        {
            if (physicsSystem.Update(deltaTime, collisionSteps, _nativeJobSystem, out var error) == false)
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
                    shape = new BoxShape(halfExtents);
                    break;
                case PlaneShapeData planeShapeData:
                    float3 normal = new(planeShapeData.normal.X, planeShapeData.normal.Y, planeShapeData.normal.Z);
                    Jolt.Plane plane = new Jolt.Plane(normal, planeShapeData.distance);
                    shape = new PlaneShape(plane, planeShapeData.halfExtent);
                    break;
                case SphereShapeData sphereShapeData:
                    shape = new SphereShape(sphereShapeData.radius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shapeData));
            }

            if (shape == null)
            {
                throw new ArgumentException($"cannot create shape settings from shape[{shapeData}] ");
            }

            rvec3 pos = new(position.X, position.Y, position.Z);
            quaternion quaternion = new(rotation.X, rotation.Y, rotation.Z, rotation.W);
            var settings = new BodyCreationSettings(
                shape.Value,
                pos,
                quaternion,
                (Jolt.MotionType)motionType,
                (uint)layers
            );
            var bodyInterface = physicsSystem.BodyInterface;

            var body = bodyInterface.CreateAndAddBody(settings, (Jolt.Activation)activation);
            
            // --------------------- //
            _bodiesSet.Add(body.ID);
            _bodies.Add(body.ID);
            // --------------------- //
            
            return body.ID;
        }

        public bool Exist(in uint id)
        {
            return _bodiesSet.Contains(id);
        }

        public bool QueryBody(in uint id, out BodyData bodyData)
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPosition(in uint id)
        {
            throw new NotImplementedException();
        }

        public Quaternion GetRotation(in uint id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBody(in uint id, in BodyData bodyData)
        {
            throw new NotImplementedException();
        }

        public void Serialize(ref WorldData worldData)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(in WorldData worldData)
        {
            throw new NotImplementedException();
        }

        public void Activate(in uint id)
        {
            throw new NotImplementedException();
        }

        public void Deactivate(in uint id)
        {
            throw new NotImplementedException();
        }

        public void RemoveAndDestroy(in uint id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            physicsSystem.Destroy();
        }
    }
}
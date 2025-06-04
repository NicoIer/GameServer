using System;
using System.Collections.Generic;
using System.Threading;
using GameCore.Jolt;
using Jolt;
using Unity.Mathematics;
using UnityEngine;
using UnityToolkit;
using Activation = GameCore.Jolt.Activation;
using MotionType = GameCore.Jolt.MotionType;
using PhysicsUpdateError = GameCore.Jolt.PhysicsUpdateError;
using Plane = Jolt.Plane;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Game.Jolt
{
    /// <summary>
    /// 世界模拟
    /// </summary>
    public class JoltPhysicsWorld : IPhysicsWorld
    {
        public byte worldId { get; private set; }

        protected readonly List<uint> _bodies = new();
        public IReadOnlyList<uint> bodies => _bodies;

        private readonly JobSystem jobSystem = JobSystem.Create(new JobSystemThreadPoolConfig());
        public PhysicsSystem physicsSystem { get; private set; }

        public delegate void SetupCollisionFilteringDelegate(ref PhysicsSystemSettings settings);


        public JoltPhysicsWorld(SetupCollisionFilteringDelegate setup)
        {
            if (!global::Jolt.Jolt.Initialized) throw new NotImplementedException("Jolt Physics Not Initialized");
            Interlocked.Increment(ref IPhysicsWorld.worldIdCounter);
            if (IPhysicsWorld.worldIdCounter > byte.MaxValue) throw new Exception("WorldId overflow");
            worldId = (byte)IPhysicsWorld.worldIdCounter;

            var settings = new PhysicsSystemSettings()
            {
                MaxBodies = IPhysicsWorld.MaxBodies,
                MaxBodyPairs = IPhysicsWorld.MaxBodyPairs,
                MaxContactConstraints = IPhysicsWorld.MaxContactConstraints,
                NumBodyMutexes = IPhysicsWorld.NumBodyMutexes,
            };
            setup(ref settings);
            physicsSystem = new PhysicsSystem(settings);
        }


        public PhysicsUpdateError Simulate(in float deltaTime, in int collisionSteps)
        {
            if (physicsSystem.Update(deltaTime, collisionSteps, jobSystem, out var error))
            {
                return (PhysicsUpdateError)error;
            }

            throw new NotImplementedException();
        }

        public uint Create(IShapeData shapeData, in Vector3 position, in Quaternion rotation, MotionType motionType,
            ObjectLayers layers, Activation activation)
        {
            // Shape? shape;
            // switch (shapeData)
            // {
            //     case BoxShapeData boxShapeData:
            //         shape = new BoxShape(boxShapeData.halfExtents.T1());
            //         break;
            //     case PlaneShapeData planeShapeData:
            //         Plane plane = new Plane(planeShapeData.normal.T1(), planeShapeData.distance);
            //         shape = new PlaneShape(plane, null, planeShapeData.halfExtent);
            //         break;
            //     case SphereShapeData sphereShapeData:
            //         shape = new SphereShape(sphereShapeData.radius);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(shapeData));
            // }
            //
            // if (shape == null)
            // {
            //     throw new ArgumentException($"cannot create shape settings from shape[{shapeData}] ");
            // }
            //
            // using var bodyCreate = new BodyCreationSettings(
            //     shape,
            //     position,
            //     rotation,
            //     (JoltPhysicsSharp.MotionType)motionType,
            //     new ObjectLayer((uint)layers)
            // );
            // var body = physicsSystem.BodyInterface.CreateAndAddBody(bodyCreate, (JoltPhysicsSharp.Activation)activation);
            // OnBodyCreated(body);
            // return body.ID;
            ShapeSettings? shapeSettings;
            switch (shapeData)
            {
                case BoxShapeData boxShapeData:
                    shapeSettings = new BoxShapeSettings(boxShapeData.halfExtents.T());
                    break;
                case PlaneShapeData planeShapeData:
                    Plane plane = new Plane(planeShapeData.normal.T(), planeShapeData.distance);
                    shapeSettings = new PlaneShapeSettings(plane, planeShapeData.halfExtent);
                    break;
                case SphereShapeData sphereShapeData:
                    shapeSettings = new SphereShapeSettings(sphereShapeData.radius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shapeData));
            }

            if (shapeSettings == null)
            {
                throw new ArgumentException($"cannot create shape settings from shape[{shapeData}] ");
            }

            var bodyCreate = new BodyCreationSettings(
                shapeSettings.Value,
                position,
                rotation.T(),
                (global::Jolt.MotionType)motionType,
                new ObjectLayer((uint)layers)
            );
            var body = physicsSystem.BodyInterface.CreateAndAddBody(bodyCreate, (global::Jolt.Activation)activation);
            OnBodyCreated(body.ID);
            return body.ID;
        }

        private void OnBodyCreated(uint bodyId)
        {
            _bodies.Add(bodyId);
        }

        public bool QueryBody(in uint id, out BodyData bodyData)
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
            jobSystem.Destroy();
            physicsSystem.Destroy();
        }
    }
}
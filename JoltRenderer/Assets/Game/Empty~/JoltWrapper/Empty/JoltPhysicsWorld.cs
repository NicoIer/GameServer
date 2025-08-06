// using System;
// using System.Collections.Generic;
// using System.Threading;
// using GameCore.Jolt;
// using Jolt;
// using Unity.Mathematics;
// using UnityEngine;
// using UnityToolkit;
// using Activation = GameCore.Jolt.Activation;
// using MotionType = GameCore.Jolt.MotionType;
// using PhysicsUpdateError = GameCore.Jolt.PhysicsUpdateError;
// using Plane = Jolt.Plane;
// using Quaternion = System.Numerics.Quaternion;
// using Vector3 = System.Numerics.Vector3;
//
// namespace JoltWrapper
// {
//     /// <summary>
//     /// 世界模拟
//     /// </summary>
//     public class JoltPhysicsWorld : IPhysicsWorld
//     {
//         public byte worldId { get; private set; }
//
//         protected readonly List<uint> _bodies = new();
//         private Dictionary<uint, BodyData> _id2bodyData = new();
//         public IReadOnlyList<uint> bodies => _bodies;
//
//         private readonly JobSystem jobSystem = JobSystem.Create(new JobSystemThreadPoolConfig());
//         public PhysicsSystem physicsSystem { get; private set; }
//
//         public delegate void OnBodyCreatedDelegate(in uint bodyId, in BodyData bodyData, in IShapeData shapeData);
//
//         public event OnBodyCreatedDelegate OnBodyCreate = delegate { };
//         public event Action OnBodyDestroy = delegate { }; // TODO 
//
//         public delegate void SetupCollisionFilteringDelegate(ref PhysicsSystemSettings settings);
//
//
//         public JoltPhysicsWorld(SetupCollisionFilteringDelegate setup)
//         {
//             if (!global::Jolt.Jolt.Initialized) throw new NotImplementedException("Jolt Physics Not Initialized");
//             Interlocked.Increment(ref IPhysicsWorld.worldIdCounter);
//             if (IPhysicsWorld.worldIdCounter > byte.MaxValue) throw new Exception("WorldId overflow");
//             worldId = (byte)IPhysicsWorld.worldIdCounter;
//
//             var settings = new PhysicsSystemSettings()
//             {
//                 MaxBodies = IPhysicsWorld.MaxBodies,
//                 MaxBodyPairs = IPhysicsWorld.MaxBodyPairs,
//                 MaxContactConstraints = IPhysicsWorld.MaxContactConstraints,
//                 NumBodyMutexes = IPhysicsWorld.NumBodyMutexes,
//             };
//             setup(ref settings);
//             physicsSystem = new PhysicsSystem(settings);
//         }
//
//
//         public PhysicsUpdateError Simulate(in float deltaTime, in int collisionSteps)
//         {
//             if (physicsSystem.Update(deltaTime, collisionSteps, jobSystem, out var error))
//             {
//                 return (PhysicsUpdateError)error;
//             }
//
//             throw new NotImplementedException();
//         }
//
//         public uint Create(IShapeData shapeData, in Vector3 position, in Quaternion rotation, MotionType motionType,
//             ObjectLayers layers, Activation activation)
//         {
//             ShapeSettings? shapeSettings;
//             switch (shapeData)
//             {
//                 case BoxShapeData boxShapeData:
//                     shapeSettings = new BoxShapeSettings(boxShapeData.halfExtents.T(), 0.05f);
//                     break;
//                 case PlaneShapeData planeShapeData:
//                     Plane plane = new Plane(planeShapeData.normal.T(), planeShapeData.distance);
//                     shapeSettings = new PlaneShapeSettings(plane, planeShapeData.halfExtent);
//                     break;
//                 case SphereShapeData sphereShapeData:
//                     shapeSettings = new SphereShapeSettings(sphereShapeData.radius);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException(nameof(shapeData));
//             }
//
//             if (shapeSettings == null)
//             {
//                 throw new ArgumentException($"cannot create shape settings from shape[{shapeData}] ");
//             }
//
//             var bodyCreate = new BodyCreationSettings(
//                 shapeSettings.Value,
//                 position,
//                 rotation.T(),
//                 (global::Jolt.MotionType)motionType,
//                 new ObjectLayer((uint)layers)
//             );
//             var body = physicsSystem.BodyInterface.CreateBody(bodyCreate);
//             physicsSystem.BodyInterface.AddBody(body.GetID(), (Jolt.Activation)activation);
//             OnBodyCreated(body.GetID().Value, shapeData);
//             return body.GetID().Value;
//         }
//
//         public bool Exist(in uint id)
//         {
//             return _bodies.Contains(id);
//         }
//
//         private void OnBodyCreated(in uint bodyId, in IShapeData shapeData)
//         {
//             bool success = QueryBody(bodyId, out var bodyData);
//             Debug.Assert(success, $"Body[{bodyId}] not found");
//             if (!success) return;
//             _bodies.Add(bodyId);
//             _id2bodyData.Add(bodyId, bodyData);
//             OnBodyCreate(in bodyId, in bodyData, in shapeData);
//         }
//
//         public bool QueryBody(in uint id, out BodyData bodyData)
//         {
//             throw new NotImplementedException();
//         }
//
//         public Vector3 GetPosition(in uint id)
//         {
//             throw new NotImplementedException();
//         }
//
//         public Quaternion GetRotation(in uint id)
//         {
//             throw new NotImplementedException();
//         }
//
//         public bool UpdateBody(in uint id, in BodyData bodyData)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Serialize(ref WorldData worldData)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Deserialize(in WorldData worldData)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Activate(in uint id)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Deactivate(in uint id)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void RemoveAndDestroy(in uint id)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Dispose()
//         {
//             jobSystem.Destroy();
//             physicsSystem.Destroy();
//         }
//     }
// }
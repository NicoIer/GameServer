// using System;
// using System.Collections.Generic;
// using Cysharp.Threading;
// using JoltWrapper;
// using GameCore.Jolt;
// using Jolt;
// using UnityEngine;
// using UnityToolkit;
// using Activation = Jolt.Activation;
// using Object = System.Object;
//
// namespace JoltWrapper
// {
//     [RequireComponent(typeof(JoltApplication))]
//     public class JoltUnity : MonoBehaviour, IJoltSystem<JoltApplication, LogicLooperActionContext, JoltPhysicsWorld>
//     {
//         private JoltApplication _app;
//         private JoltPhysicsWorld _world;
//         public bool autoSyncUnity2JoltTransform = true; // 自动将Unity Transform同步到Jolt
//         public bool autoSyncJolt2UnityTransform = true; // 自动将Jolt Transform同步到Unity
//
//         private Dictionary<uint, PhysicsBody> _unityBodies = new Dictionary<uint, PhysicsBody>(16);
//
//         private Dictionary<ShapeTypeEnum, ShapeCreator> shapeCreators = new Dictionary<ShapeTypeEnum, ShapeCreator>();
//
//
//         public bool autoBindBodies = true;
//         
//
//         private void Awake()
//         {
//             foreach (var shapeCreator in GetComponents<ShapeCreator>())
//             {
//                 shapeCreators[shapeCreator.GetShapeType()] = shapeCreator;
//             }
//         }
//
//         public void OnAdded(JoltApplication app, JoltPhysicsWorld world)
//         {
//             _app = app;
//             _app.physicsWorld.OnBodyCreate += OnBodyCreated;
//
//             _world = world;
//         }
//
//
//         public void OnRemoved()
//         {
//             _app.physicsWorld.OnBodyCreate -= OnBodyCreated;
//         }
//
//
//         private void OnBodyCreated(in uint bodyId, in BodyData bodyData, in IShapeData shapeData)
//         {
//             // 创建一个Unity对象和Native对应
//             var physicsShape = shapeCreators[shapeData.shapeType].Create(in bodyData, in shapeData);
//             var physicsBody = physicsShape.body;
//             physicsBody.OnInit(bodyId, bodyData, shapeData, physicsShape);
//             _unityBodies[bodyId] = physicsBody;
//         }
//
//         public void BeforePhysicsStart()
//         {
//             var bodies = FindObjectsByType<PhysicsBody>(FindObjectsSortMode.None);
//             if (autoBindBodies)
//             {
//                 foreach (var physicsBody in bodies)
//                 {
//                     var shape = physicsBody.GetComponent<IPhysicsShape>();
//                     if (shape == null)
//                     {
//                         Destroy(physicsBody);
//                         continue;
//                     }
//
//                     var shapeData = shape.iShapeData;
//                     _world.Create(
//                         shapeData,
//                         physicsBody.transform.position.T(),
//                         physicsBody.transform.rotation.T(),
//                         physicsBody.motionType,
//                         physicsBody.layers,
//                         physicsBody.activation
//                     );
//                 }
//             }
//             else
//             {
//                 foreach (var physicsBody in bodies)
//                 {
//                     Destroy(physicsBody);
//                 }
//             }
//         }
//
//         public void BeforePhysicsUpdate(in LogicLooperActionContext ctx)
//         {
//             if (autoSyncUnity2JoltTransform)
//             {
//                 foreach (var physicsBody in _unityBodies.Values)
//                 {
//                     var bodyTransform = physicsBody.transform;
//                     _world.physicsSystem.BodyInterface.SetPositionAndRotation(physicsBody.id,
//                         bodyTransform.position.T(),
//                         bodyTransform.rotation, (Activation)physicsBody.activation);
//                 }
//             }
//         }
//
//         public void AfterPhysicsUpdate(in LogicLooperActionContext ctx)
//         {
//             if (autoSyncJolt2UnityTransform)
//             {
//                 foreach (var physicsBody in _unityBodies.Values)
//                 {
//                     bool ok = _world.QueryBody(physicsBody.id, out var data);
//                     Debug.Assert(ok, $"Body[{physicsBody.id}]不存在!");
//                     transform.position = data.position.T();
//                     transform.rotation = data.rotation.T();
//                 }
//             }
//         }
//
//         public void AfterPhysicsStop()
//         {
//             // throw new System.NotImplementedException();
//         }
//
//         public void Dispose()
//         {
//             // throw new System.NotImplementedException();
//         }
//     }
// }
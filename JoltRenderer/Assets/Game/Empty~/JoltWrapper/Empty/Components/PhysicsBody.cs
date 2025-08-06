// using System;
// using System.Runtime.CompilerServices;
// using GameCore.Jolt;
// using UnityEngine;
// using UnityToolkit;
//
// namespace JoltWrapper
// {
//     public class PhysicsBody : MonoBehaviour
//     {
//         private IPhysicsWorld _physicsWorld;
//         public new Transform transform { get; private set; }
//         public uint id { get; private set; }
//
//         public bool initialized { get; private set; }
//
//         [field: SerializeField] public MotionType motionType { get; private set; }
//         [field: SerializeField] public Activation activation { get; private set; }
//         [field: SerializeField] public ObjectLayers layers { get; private set; }
//
//         public System.Numerics.Vector3 position
//         {
//             [MethodImpl(MethodImplOptions.AggressiveInlining)]
//             get => _physicsWorld.GetPosition(id);
//         }
//
//         public System.Numerics.Quaternion rotation
//         {
//             [MethodImpl(MethodImplOptions.AggressiveInlining)]
//             get => _physicsWorld.GetRotation(id);
//         }
//
//
//         private void Awake()
//         {
//             transform = gameObject.transform;
//         }
//
//         public virtual void OnInit(in uint bodyId, in BodyData bodyData, in IShapeData shapeData,
//             IPhysicsShape physicsShape)
//         {
//             id = bodyId;
//
//
//             initialized = true;
//         }
//     }
// }
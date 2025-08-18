// using System;
// using GameCore.Jolt;
// using Jolt;
// using UnityEngine;
//
// namespace JoltWrapper
// {
//     public interface IPhysicsShape
//     {
//         internal IShapeData iShapeData { get; }
//         internal PhysicsBody body { get; }
//     }
//
//     [RequireComponent(typeof(PhysicsBody))]
//     public abstract class PhysicsShape<TShapeData> : MonoBehaviour, IPhysicsShape where TShapeData : IShapeData
//     {
//         IShapeData IPhysicsShape.iShapeData => shapeData;
//
//         PhysicsBody IPhysicsShape.body => body;
//
//        public PhysicsBody body { get; private set; }
//
//        [field: SerializeField] 
//         public TShapeData shapeData { get; protected set; }
//
//         public virtual void Bind(in TShapeData boxShapeData, in PhysicsBody body)
//         {
//             shapeData = boxShapeData;
//             this.body = body;
//         }
//     }
// }
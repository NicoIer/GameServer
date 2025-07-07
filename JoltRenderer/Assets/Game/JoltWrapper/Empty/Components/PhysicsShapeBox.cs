// using GameCore.Jolt;
// using Jolt;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityToolkit;
//
// namespace JoltWrapper
// {
//     public class PhysicsShapeBox : PhysicsShape<BoxShapeData>
//     {
//         public float convexRadius { get; private set; } = PhysicsSettings.DefaultConvexRadius;
//         public Vector3 halfExtents { get; private set; } = new Vector3(1, 1, 1);
//         public override void Bind(in BoxShapeData boxShapeData, in PhysicsBody body)
//         {
//             base.Bind(in boxShapeData, in body);
//             halfExtents = boxShapeData.halfExtents.T();
//         }
//     }
// }
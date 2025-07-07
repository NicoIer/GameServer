// using System;
// using Unity.Mathematics;
// using UnityEditor;
//
// namespace JoltWrapper.Editor
// {
//     [CustomEditor(typeof(PhysicsShapeBox)), CanEditMultipleObjects]
//     public class PhysicsShapeBoxEditor : UnityEditor.Editor
//     {
//         private void OnSceneGUI()
//         {
//             var shape = target as PhysicsShapeBox;
//             if (shape.body == null) return;
//             if (!shape.body.initialized) return;
//             var pos = shape.body.position;
//             var rot = shape.body.rotation;
//             PhysicsShapeHandles.DrawBoxShape(new float3(pos.X, pos.Y, pos.Z),
//                 new quaternion(rot.X, rot.Y, rot.Z, rot.W), shape);
//         }
//     }
// }
// using GameCore.Jolt;
// using UnityEngine;
//
// namespace JoltWrapper
// {
//     public class BoxShapeCreator : ShapeCreator
//     {
//         public override ShapeTypeEnum GetShapeType() => ShapeTypeEnum.Box;
//         // public PhysicsShapeBox prefab;
//
//         public override IPhysicsShape Create(in BodyData bodyData, in IShapeData shapeData)
//         {
//             if (shapeData is not BoxShapeData boxShapeData)
//             {
//                 throw new System.Exception("Box shape data is not BoxShapeData");
//             }
//             var obj = new GameObject($"Box[{bodyData.entityId}]");
//             var body = obj.AddComponent<PhysicsBody>();
//             var shape = obj.AddComponent<PhysicsShapeBox>();
//             shape.Bind(in boxShapeData, body);
//             return shape;
//         }
//     }
// }
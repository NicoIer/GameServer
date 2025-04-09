using GameCore.Jolt;
using UnityEngine;
using UnityToolkit;

namespace Game.Jolt
{
    public class JoltBoxShape :  JoltShape<BoxShapeData>
    {
        public override void OnInit(in BoxShapeData data, in JoltBody refBody)
        {
            base.OnInit(in data, in refBody);
        }

        public override void OnShapeUpdate(in ShapeDataPacket bodyDataShapeDataPacket)
        {
            base.OnShapeUpdate(in bodyDataShapeDataPacket);
        }
    }
}
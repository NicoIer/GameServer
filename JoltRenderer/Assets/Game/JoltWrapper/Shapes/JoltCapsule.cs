using System;
using GameCore.Physics;
using UnityEngine;

namespace JoltWrapper
{
    public class JoltCapsule : JoltShape
    {
        public float halfHeight = 0.5f;
        public float radius = 0.5f;
        public override IShapeData shapeData => new CapsuleShapeData(radius, halfHeight);
    }
}
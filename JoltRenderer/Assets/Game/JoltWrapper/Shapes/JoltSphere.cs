using System;
using GameCore.Physics;
using UnityEngine;

namespace JoltWrapper
{
    public class JoltSphere : JoltShape
    {
        public float radius = 0.5f;
        public override IShapeData shapeData => new SphereShapeData(radius);
        private void OnValidate()
        {
            if (radius <= 0)
            {
                Debug.LogWarning("Sphere shape radius must be positive.", this);
            }
        }
    }
}
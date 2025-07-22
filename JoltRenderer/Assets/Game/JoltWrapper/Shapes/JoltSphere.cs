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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position + centerOffset, transform.rotation, new Vector3(1, 1, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
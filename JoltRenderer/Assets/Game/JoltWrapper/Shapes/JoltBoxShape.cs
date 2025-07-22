using System;
using GameCore.Physics;
using UnityEngine;
using UnityToolkit;

namespace JoltWrapper
{
    public class JoltBoxShape : JoltShape
    {
        [SerializeField]
        private Vector3 halfExtents = Vector3.one;
        

        public override IShapeData shapeData => new BoxShapeData(halfExtents.T());
        private void OnValidate()
        {
            if (halfExtents.x <= 0 || halfExtents.y <= 0 || halfExtents.z <= 0)
            {
                Debug.LogWarning("Box shape half extents must be positive.", this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position + centerOffset, transform.rotation, halfExtents * 2);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
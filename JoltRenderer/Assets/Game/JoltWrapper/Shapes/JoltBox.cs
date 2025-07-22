using System;
using GameCore.Physics;
using Jolt;
using UnityEngine;
using UnityToolkit;
using AllowedDOFs = GameCore.Physics.AllowedDOFs;

namespace JoltWrapper
{
    public class JoltBox : JoltShape
    {
        [field: SerializeField] public Vector3 halfExtents { get; private set; } = Vector3.one;
        public float ConvexRadius = PhysicsSettings.DefaultConvexRadius;
        public AllowedDOFs allowedDOFs = AllowedDOFs.All;
        public override IShapeData shapeData => new BoxShapeData(halfExtents.T(), ConvexRadius)
        {
            allowedDoFs = allowedDOFs
        };

        private void OnValidate()
        {
            if (halfExtents.x <= 0 || halfExtents.y <= 0 || halfExtents.z <= 0)
            {
                Debug.LogWarning("Box shape half extents must be positive.", this);
            }
        }
    }
}
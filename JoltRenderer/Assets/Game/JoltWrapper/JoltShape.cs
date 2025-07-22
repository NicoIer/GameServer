using GameCore.Physics;
using UnityEngine;

namespace JoltWrapper
{
    
    [RequireComponent(typeof(JoltBody))]
    public abstract class JoltShape : MonoBehaviour
    {
        public Vector3 centerOffset;
        public abstract IShapeData shapeData { get; }
    }
}
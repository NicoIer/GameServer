using GameCore.Physics;
using UnityEngine;

namespace JoltWrapper
{
    public abstract class JoltShape : MonoBehaviour
    {
        public abstract IShapeData shapeData { get; }
    }
}
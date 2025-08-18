using GameCore.Physics;
using UnityEngine;

namespace JoltWrapper
{

    [RequireComponent(typeof(JoltBody))]
    public abstract class JoltShape : MonoBehaviour
    {
        public abstract IShapeData shapeData { get; }
        private JoltBody _body;

        public JoltBody body
        {
            get
            {
                _body ??= GetComponent<JoltBody>();
                return _body;
                
            }
        }

    }
}
using GameCore.Jolt;
using UnityEngine;
using UnityToolkit;

namespace Game.Jolt
{
    public class JoltBody : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
        private BodyData _currentData;
        
        public BodyType bodyType;

        public void OnWorldUpdate(in BodyData body)
        {
            _currentData = body;
            transform.position = body.position.T();
            transform.rotation = body.rotation.T();
        }
    }
}
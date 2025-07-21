using System;
using UnityEngine;

namespace JoltWrapper
{
    public class JoltApplication : MonoBehaviour
    {
        public JoltPhysicsWorld physicsWorld { get; private set; }

        private void Awake()
        {
            
        }

        private void OnDestroy()
        {
        }
    }
}
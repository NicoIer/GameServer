using System;
using UnityEngine;

namespace JoltWrapper
{
    public class JoltApplication : MonoBehaviour
    {
        public JoltPhysicsWorld physicsWorld { get; private set; }

        private void Awake()
        {
            physicsWorld = new JoltPhysicsWorld();
        }

        private void FixedUpdate()
        {
            physicsWorld.Simulate(Time.fixedDeltaTime, 1);
        }

        private void OnDestroy()
        {
            physicsWorld.Dispose();
        }
    }
}
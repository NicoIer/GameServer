using System;
using UnityEngine;

namespace JoltWrapper
{
    public class JoltApplication : MonoBehaviour
    {
        public JoltPhysicsWorld physicsWorld { get; private set; }
        public event Action BeforeSimulation;
        public event Action AfterSimulation;
        public const int CollisionStep = 1;
        public event Action BeforeOptimization;

        private void Start()
        {
            physicsWorld = new JoltPhysicsWorld();
            BeforeOptimization?.Invoke();
            physicsWorld.physicsSystem.OptimizeBroadPhase();
        }

        private void FixedUpdate()
        {
            BeforeSimulation?.Invoke();
            physicsWorld.Simulate(Time.fixedDeltaTime, CollisionStep);
            AfterSimulation?.Invoke();
        }

        private void OnDestroy()
        {
            physicsWorld.Dispose();
        }
    }
}
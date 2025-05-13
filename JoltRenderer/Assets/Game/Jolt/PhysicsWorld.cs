using System;
using System.Collections.Generic;
using GameCore.Jolt;
using UnityEngine;
using UnityToolkit;

namespace Game.Jolt
{
    /// <summary>
    /// 世界模拟
    /// </summary>
    public class PhysicsWorld : MonoBehaviour
    {
        public WorldData current;

        public void Simulate(float deltaTime)
        {
        }

        public void RollBack(in byte frameCount)
        {
        }
    }
}
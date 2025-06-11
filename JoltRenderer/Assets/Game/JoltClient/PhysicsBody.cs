using System;
using GameCore.Jolt;
using UnityEngine;

namespace Game.JoltClient
{
    public class PhysicsBody : MonoBehaviour
    {
        public BodyData local;
        [NonSerialized] public BodyData remote;
        
    }
}
using System;
using GameCore.Physics;
using Jolt;
using UnityEngine;
using Activation = GameCore.Physics.Activation;
using MotionType = GameCore.Physics.MotionType;

namespace JoltWrapper
{
    // [RequireComponent(typeof(JoltShape))]
    public class JoltBody : MonoBehaviour
    {
        public BodyID bodyID { get; private set; }
        public JoltPhysicsWorld physicsWorld { get; private set; }
        public JoltShape shape;

        public MotionType motionType = MotionType.Dynamic;
        public ObjectLayers objectLayers = ObjectLayers.Moving;
        public Activation activation = Activation.Activate;


        internal void BindNative(BodyID bodyID, JoltPhysicsWorld physicsWorld)
        {
            this.bodyID = bodyID;
            this.physicsWorld = physicsWorld;
        }

        private void OnValidate()
        {
            shape = GetComponent<JoltShape>();
        }
    }
}
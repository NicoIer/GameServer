using System;
using GameCore.Physics;
using Jolt;
using UnityEngine;
using Activation = GameCore.Physics.Activation;
using MotionType = GameCore.Physics.MotionType;

namespace JoltWrapper
{
    // [RequireComponent(typeof(JoltShape))]
    [AddComponentMenu("")]
    public class JoltBody : MonoBehaviour
    {
        public BodyID bodyID { get; private set; }
        public JoltPhysicsWorld physicsWorld { get; private set; }
        private JoltShape _shape;

        public JoltShape shape
        {
            get
            {
                if (_shape == null)
                {
                    _shape = GetComponent<JoltShape>();
                }

                if (_shape == null)
                {
                    throw new InvalidOperationException("JoltBody requires a JoltShape component.");
                }

                return _shape;
            }
        }

        public MotionType motionType = MotionType.Dynamic;
        public ObjectLayers objectLayers = ObjectLayers.Moving;
        public Activation activation = Activation.Activate;

        public Vector3 position => transform.position;

        public Quaternion rotation => transform.rotation;


        internal void BindNative(BodyID bodyID, JoltPhysicsWorld physicsWorld)
        {
            this.bodyID = bodyID;
            this.physicsWorld = physicsWorld;
        }
    }
}
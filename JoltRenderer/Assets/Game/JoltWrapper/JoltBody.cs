using System;
using GameCore.Physics;
using Jolt;
using UnityEngine;
using Activation = GameCore.Physics.Activation;
using AllowedDOFs = GameCore.Physics.AllowedDOFs;
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

        /// <summary>
        /// 物理坐标
        /// </summary>
        [field: SerializeField]
        public Vector3 position { get; internal set; }

        /// <summary>
        /// 物理旋转
        /// </summary>
        [field: SerializeField]
        public Quaternion rotation { get; internal set; }
        
        
        
        internal bool setPositionAndRotationThisSimulation { get; set; } = false;
        internal Quaternion setRotationThisSimulation { get; set; }
        internal Vector3 setPositionThisSimulation { get; set; }

        private void OnValidate()
        {
            position = transform.position;
            rotation = transform.rotation;
        }


        internal void BindNative(BodyID bodyID, JoltPhysicsWorld physicsWorld)
        {
            this.bodyID = bodyID;
            this.physicsWorld = physicsWorld;
        }

        public void SetPositionAndRotation(Vector3 vector3, Quaternion quaternion)
        {
            setPositionAndRotationThisSimulation = true;
            setPositionThisSimulation = vector3;
            setRotationThisSimulation = quaternion;
        }
    }
}
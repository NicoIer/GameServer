using System;
using GameCore.Jolt;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game.Jolt
{
    public class JoltBody : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
#endif
        public BodyData remoteData { get; private set; }

        public BodyType bodyType;
        public bool isActive;
        public MotionType motionType;
        public bool isSensor;
        public ObjectLayers objectLayer;
        public float friction;
        public float restitution;

        public
            // System.Numerics.
            Vector3 position;

        public
            // System.Numerics.
            Quaternion rotation;

        public
            // System.Numerics.
            Vector3 linearVelocity;

        public
            // System.Numerics.
            Vector3 angularVelocity;

        public IJoltShape shape { get; private set; }
        
        public void OnBodyDataUpdate(in BodyData body)
        {
            remoteData = body;

            bodyType = body.bodyType;
            isActive = body.isActive;
            motionType = body.motionType;
            isSensor = body.isSensor;
            objectLayer = (ObjectLayers)body.objectLayer;
            friction = body.friction;
            restitution = body.restitution;

            position = body.position.T();
            rotation = body.rotation.T();
            linearVelocity = body.linearVelocity.T();
            angularVelocity = body.angularVelocity.T();

            SyncTransform();
        }

        private void SyncTransform()
        {
            transform.position = position;
            transform.rotation = rotation;
        }

        public void CreateShape<TShapeData, TJoltShape>(in TShapeData shapeData) where TShapeData : IShapeData
            where TJoltShape : JoltShape<TShapeData>, new()
        {
            Assert.IsNull(shape);
            var joltShape = new TJoltShape();
            joltShape.OnInit(shapeData, this);
            shape = joltShape;
            Assert.IsNotNull(shape);
        }
    }
}
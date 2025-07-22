using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityToolkit;

namespace JoltWrapper
{
    [RequireComponent(typeof(JoltApplication))]
    public class JoltSceneAutoBinder : MonoBehaviour
    {
        private JoltApplication _application;
        public List<JoltBody> managedBodyList = new List<JoltBody>();

        private void Awake()
        {
            _application = GetComponent<JoltApplication>();
            _application.BeforeOptimization += InitializeJoltScene;
            _application.BeforeSimulation += BeforeSimulation;
            _application.AfterSimulation += AfterSimulation;
        }

        private void BeforeSimulation()
        {
            
        }
        
        private void AfterSimulation()
        {
            var bodyInterface = _application.physicsWorld.physicsSystem.BodyInterface;
            foreach (var joltBody in managedBodyList)
            {
                var wt = bodyInterface.GetWorldTransform(joltBody.bodyID);
                var pos = wt.c3.xyz;
                var rot = new quaternion(wt);
                joltBody.transform.SetPositionAndRotation(pos, rot);
            }
        }


        private void InitializeJoltScene()
        {
            var managedBodies = FindObjectsByType<JoltBody>(FindObjectsSortMode.None);
            foreach (var body in managedBodies)
            {
                var bodyId = _application.physicsWorld.CreateAndAdd(
                    body.shape.shapeData,
                    (body.transform.position + body.shape.centerOffset).T(),
                    body.transform.rotation.T(),
                    body.motionType,
                    body.objectLayers,
                    body.activation
                );
                body.BindNative(bodyId, _application.physicsWorld);
                managedBodyList.Add(body);
            }
        }
    }
}
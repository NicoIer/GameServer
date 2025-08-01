using System;
using System.Collections.Generic;
using Jolt;
using Unity.Mathematics;
using UnityEngine;
using UnityToolkit;

namespace JoltWrapper
{
    [RequireComponent(typeof(JoltApplication))]
    public class JoltSceneBinder : MonoBehaviour
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
            var bodyInterface = _application.physicsWorld.physicsSystem.BodyInterface;
            foreach (var joltBody in managedBodyList)
            {
                // 如果需要将Unity中的位置和旋转同步到物理世界
                if (joltBody.setPositionAndRotationThisSimulation)
                {
                    bodyInterface.SetPositionAndRotationWhenChanged(
                        joltBody.bodyID,
                        joltBody.setPositionThisSimulation,
                        joltBody.setRotationThisSimulation,
                        Activation.Activate
                    );
                    joltBody.setPositionAndRotationThisSimulation = false; // 同步后不再需要
                }
            }
        }

        private void AfterSimulation()
        {
            var bodyInterface = _application.physicsWorld.physicsSystem.BodyInterface;
            foreach (var joltBody in managedBodyList)
            {
                // 将物理世界中的位置和旋转同步到Unity
                var wt = bodyInterface.GetWorldTransform(joltBody.bodyID);
                var pos = wt.c3.xyz;
                var rot = new quaternion(wt);
                joltBody.position = pos;
                joltBody.rotation = rot;
                joltBody.transform.SetPositionAndRotation(pos, rot);
            }
        }


        private void InitializeJoltScene()
        {
            var managedBodies = FindObjectsByType<JoltBody>(FindObjectsSortMode.None);
            var bodyInterface = _application.physicsWorld.physicsSystem.BodyInterface;
            var bodyLockInterface = _application.physicsWorld.physicsSystem.GetBodyLockInterface();
            foreach (var body in managedBodies)
            {
                // unsafe
                // {
                body.transform.position = body.position;
                body.transform.rotation = body.rotation;

                var bodyId = _application.physicsWorld.CreateAndAdd(
                    body.shape.shapeData,
                    body.position.T(),
                    body.rotation.T(),
                    body.motionType,
                    body.objectLayers,
                    body.activation
                );

                // JPH_BodyLockWrite write;
                // UnsafeBindings.JPH_BodyLockInterface_LockWrite(bodyLockInterface.Handle, bodyId, &write);
                // UnsafeBindings.JPH_Shape_GetMassProperties();
                // UnsafeBindings.JPH_BodyLockInterface_UnlockWrite(bodyLockInterface.Handle, &write);
                //
                body.BindNative(bodyId, _application.physicsWorld);
                managedBodyList.Add(body);
                // }
            }
        }
    }
}
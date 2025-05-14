using System;
using GameCore.Jolt;
using UnityEngine;
using UnityToolkit;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Game.Jolt
{
    [RequireComponent(typeof(JoltClient))]
    public class JoltCmd : MonoBehaviour
    {
        private JoltClient _client;

        private void Start()
        {
            _client = GetComponent<JoltClient>();
        }

        private void OnDestroy()
        {
        }

        [Sirenix.OdinInspector.Button]
        private void CmdSpawnBox(System.Numerics.Vector3 halfExtents, System.Numerics.Vector3 position,
            MotionType motionType = MotionType.Dynamic,
            Activation activation = Activation.Activate,
            ObjectLayers objectLayer = ObjectLayers.Moving
        )
        {
            CmdSpawnBox cmd = new CmdSpawnBox
            {
                halfExtents = halfExtents,
                position = position,
                rotation = System.Numerics.Quaternion.Identity,
                motionType = motionType,
                activation = activation,
                objectLayer = objectLayer
            };
            _client.Send(cmd);
        }

        [Sirenix.OdinInspector.Button]
        private void CmdSpawnPlane(
            // float distance = 0,
            Vector3 position,
            Vector3 rotation,
            float halfExtent = 10,
            MotionType motionType = MotionType.Static,
            Activation activation = Activation.Activate,
            ObjectLayers objectLayer = ObjectLayers.NonMoving
        )
        {
            // UnityEngine.Vector3 normal = new UnityEngine.Vector3(0, 1, 0);
            // UnityEngine.Vector3 position = normal * distance;
            // 根据法线计算旋转
            // UnityEngine.Quaternion rotation = UnityEngine.Quaternion.FromToRotation(UnityEngine.Vector3.up, normal);
            CmdSpawnPlane cmd = new CmdSpawnPlane
            {
                position = position,
                rotation = UnityEngine.Quaternion.Euler(rotation.T()).T(),
                normal = new Vector3(0,1,0),
                distance = 0,
                halfExtent = halfExtent,
                motionType = motionType,
                activation = activation,
                objectLayer = objectLayer
            };
            _client.Send(cmd);
        }
    }
}
using System;
using GameCore.Physics;
using Jolt;
using UnityEngine;
using UnityToolkit;
using Activation = GameCore.Physics.Activation;
using MotionType = GameCore.Physics.MotionType;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace Network.Physics
{
    [RequireComponent(typeof(NetworkCenter))]
    public class NetworkCmd : MonoBehaviour
    {
        private NetworkCenter _client;

        private void Start()
        {
            _client = GetComponent<NetworkCenter>();
        }

        private void OnDestroy()
        {
        }

        [Sirenix.OdinInspector.Button]
        private void CmdSpawnBox(Vector3 halfExtents, Vector3 position,
            float convexRadius = PhysicsSettings.DefaultConvexRadius,
            MotionType motionType = MotionType.Dynamic,
            Activation activation = Activation.Activate,
            ObjectLayers objectLayer = ObjectLayers.Moving
        )
        {
            var shape = new BoxShapeData(halfExtents, convexRadius);
            ShapeDataPacket.Create(shape, out var packet);
            CmdSpawnBody cmd = new CmdSpawnBody
            {
                shapeDataPacket = packet,
                position = position,
                rotation = Quaternion.Identity,
                motionType = motionType,
                activation = activation,
                objectLayer = objectLayer
            };
            _client.Send(cmd);
        }

        // [Sirenix.OdinInspector.Button]
        // private void CmdSpawnPlane(
        //     Vector3 position,
        //     Vector3 rotation,
        //     float halfExtent = 10,
        //     MotionType motionType = MotionType.Static,
        //     Activation activation = Activation.Activate,
        //     ObjectLayers objectLayer = ObjectLayers.NonMoving
        // )
        // {
        //     var shape = new PlaneShapeData(halfExtent, new Vector3(0, 1, 0), 0);
        //     ShapeDataPacket.Create(shape, out var packet);
        //     CmdSpawnBody cmd = new CmdSpawnBody
        //     {
        //         shapeDataPacket = packet,
        //         position = position,
        //         rotation = UnityEngine.Quaternion.Euler(rotation.T()).T(),
        //         motionType = motionType,
        //         activation = activation,
        //         objectLayer = objectLayer
        //     };
        //     _client.Send(cmd);
        // }
    }
}
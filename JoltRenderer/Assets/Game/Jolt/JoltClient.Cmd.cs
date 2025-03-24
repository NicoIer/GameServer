using System.Numerics;
using GameCore.Jolt;

namespace Game.Jolt
{
    public partial class JoltClient
    {
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
            Vector3 position,
            float distance = 0,
            float halfExtent = 10,
            MotionType motionType = MotionType.Static,
            Activation activation = Activation.Activate,
            ObjectLayers objectLayer = ObjectLayers.NonMoving
        )
        {
            CmdSpawnPlane cmd = new CmdSpawnPlane
            {
                position = position,
                rotation = Quaternion.Identity,
                normal = new Vector3(0, 1, 0),
                distance = distance,
                halfExtent = halfExtent,
                motionType = motionType,
                activation = activation,
                objectLayer = objectLayer
            };
            _client.Send(cmd);
        }
    }
}
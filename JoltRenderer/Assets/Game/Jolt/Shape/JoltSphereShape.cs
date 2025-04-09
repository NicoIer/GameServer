using GameCore.Jolt;

namespace Game.Jolt
{
    public class JoltSphereShape:JoltShape<SphereShapeData>
    {
        public override void OnInit(in SphereShapeData data, in JoltBody refBody)
        {
            base.OnInit(in data, in refBody);
        }

        public override void OnShapeUpdate(in ShapeDataPacket bodyDataShapeDataPacket)
        {
            base.OnShapeUpdate(in bodyDataShapeDataPacket);
        }
    }
}
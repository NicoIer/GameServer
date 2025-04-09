using GameCore.Jolt;

namespace Game.Jolt
{
    public class JoltPlaneShape : JoltShape<PlaneShapeData>
    {
        public override void OnInit(in PlaneShapeData data, in JoltBody refBody)
        {
            base.OnInit(in data, in refBody);
        }

        public override void OnShapeUpdate(in ShapeDataPacket bodyDataShapeDataPacket)
        {
            base.OnShapeUpdate(in bodyDataShapeDataPacket);
        }
    }
}
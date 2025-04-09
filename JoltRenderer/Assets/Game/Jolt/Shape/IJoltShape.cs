using System;
using GameCore.Jolt;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game.Jolt
{
    public interface IJoltShape
    {
        void OnShapeUpdate(in ShapeDataPacket bodyDataShapeDataPacket);
    }


    public class JoltShape<TShapeData> : IJoltShape where TShapeData : IShapeData
    {
        public TShapeData shapeData { get; private set; }
        public JoltBody refBody { get; private set; }

        public virtual void OnInit(in TShapeData data, in JoltBody refBody)
        {
            this.shapeData = data;
            this.refBody = refBody;
        }

        public virtual void OnShapeUpdate(in ShapeDataPacket bodyDataShapeDataPacket)
        {
            var newData = ShapeDataPacket.Deserialize(bodyDataShapeDataPacket);
            if (newData is TShapeData newShapeData)
            {
                this.shapeData = newShapeData;
            }
            else
            {
                ToolkitLog.Exception(
                    new ArgumentException($"{typeof(TShapeData)} is not match to {newData.GetType()}"));
            }
        }
    }
}
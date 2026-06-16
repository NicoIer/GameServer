// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class OffsetCenterOfMassShape : Shape
    {
        public OffsetCenterOfMassShape(Float3 offset, Shape innerShape)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_OffsetCenterOfMassShape_Create(
                    (JPH_Vec3*)&offset, (JPH_Shape*)innerShape.Handle);
            }
        }

        public Float3 Offset
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_OffsetCenterOfMassShape_GetOffset((JPH_OffsetCenterOfMassShape*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }
    }
}

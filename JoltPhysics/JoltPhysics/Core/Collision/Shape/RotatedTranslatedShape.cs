// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class RotatedTranslatedShape : Shape
    {
        public RotatedTranslatedShape(Float3 position, Quat rotation, Shape innerShape)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_RotatedTranslatedShape_Create(
                    (JPH_Vec3*)&position, (JPH_Quat*)&rotation, (JPH_Shape*)innerShape.Handle);
            }
        }

        public Float3 Position
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_RotatedTranslatedShape_GetPosition((JPH_RotatedTranslatedShape*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }

        public Quat Rotation
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Quat result;
                    Methods.JPH_RotatedTranslatedShape_GetRotation((JPH_RotatedTranslatedShape*)Handle, (JPH_Quat*)&result);
                    return result;
                }
            }
        }
    }
}

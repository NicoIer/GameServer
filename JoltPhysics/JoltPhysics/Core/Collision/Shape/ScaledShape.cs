// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class ScaledShape : Shape
    {
        public ScaledShape(Shape innerShape, Float3 scale)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                var nativeScale = new JPH_Vec3 { x = scale.x, y = scale.y, z = scale.z };
                Handle = (IntPtr)Methods.JPH_ScaledShape_Create(
                    (JPH_Shape*)innerShape.Handle, &nativeScale);
            }
        }

        public Float3 Scale
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_Vec3 result;
                    Methods.JPH_ScaledShape_GetScale((JPH_ScaledShape*)Handle, &result);
                    return new Float3(result.x, result.y, result.z);
                }
            }
        }
    }
}

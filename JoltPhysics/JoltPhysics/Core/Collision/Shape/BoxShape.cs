// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    public sealed class BoxShape: Shape
    {
        public BoxShape(Float3 halfExtent, float convexRadius = 0.05f)
                : base(IntPtr.Zero)
        {
            unsafe
            {
                var native = new JPH_Vec3 { x = halfExtent.x, y = halfExtent.y, z = halfExtent.z };
                Handle = (IntPtr)Methods.JPH_BoxShape_Create(&native, convexRadius);
            }
        }

        public Float3 HalfExtent
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_Vec3 result;
                    Methods.JPH_BoxShape_GetHalfExtent((JPH_BoxShape*)Handle, &result);
                    return new Float3(result.x, result.y, result.z);
                }
            }
        }
    }
}
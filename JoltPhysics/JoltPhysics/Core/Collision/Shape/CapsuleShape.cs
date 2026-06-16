// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    public sealed class CapsuleShape: Shape
    {
        public CapsuleShape(float halfHeightOfCylinder, float radius)
                : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_CapsuleShape_Create(halfHeightOfCylinder, radius);
            }
        }

        public float Radius
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_CapsuleShape_GetRadius((JPH_CapsuleShape*)Handle);
                }
            }
        }

        public float HalfHeightOfCylinder
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_CapsuleShape_GetHalfHeightOfCylinder((JPH_CapsuleShape*)Handle);
                }
            }
        }
    }
}
// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    public sealed class CylinderShape : Shape
    {
        public CylinderShape(float halfHeight, float radius)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_CylinderShape_Create(halfHeight, radius);
            }
        }

        public float Radius
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_CylinderShape_GetRadius((JPH_CylinderShape*)Handle); }
            }
        }

        public float HalfHeight
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_CylinderShape_GetHalfHeight((JPH_CylinderShape*)Handle); }
            }
        }
    }
}

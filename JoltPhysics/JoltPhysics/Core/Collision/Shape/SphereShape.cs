// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{

    public sealed class SphereShape : Shape
    {
        public SphereShape(float radius)
                : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_SphereShape_Create(radius);
            }
        }

        public float Radius
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SphereShape_GetRadius((JPH_SphereShape*)Handle);
                }
            }
        }
    }
}


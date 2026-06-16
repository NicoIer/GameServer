// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class TaperedCapsuleShape : Shape
    {
        public TaperedCapsuleShape(float halfHeight, float topRadius, float bottomRadius)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                var settings = Methods.JPH_TaperedCapsuleShapeSettings_Create(halfHeight, topRadius, bottomRadius);
                Handle = (IntPtr)Methods.JPH_TaperedCapsuleShapeSettings_CreateShape(settings);
                Methods.JPH_ShapeSettings_Destroy((JPH_ShapeSettings*)settings);
            }
        }

        public float TopRadius
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TaperedCapsuleShape_GetTopRadius((JPH_TaperedCapsuleShape*)Handle); }
            }
        }

        public float BottomRadius
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TaperedCapsuleShape_GetBottomRadius((JPH_TaperedCapsuleShape*)Handle); }
            }
        }

        public float HalfHeight
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TaperedCapsuleShape_GetHalfHeight((JPH_TaperedCapsuleShape*)Handle); }
            }
        }
    }
}

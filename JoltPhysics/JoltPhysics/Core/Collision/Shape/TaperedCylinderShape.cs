// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class TaperedCylinderShape : Shape
    {
        public TaperedCylinderShape(float halfHeight, float topRadius, float bottomRadius, float convexRadius = 0.05f)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                var settings = Methods.JPH_TaperedCylinderShapeSettings_Create(halfHeight, topRadius, bottomRadius, convexRadius, null);
                Handle = (IntPtr)Methods.JPH_TaperedCylinderShapeSettings_CreateShape(settings);
                Methods.JPH_ShapeSettings_Destroy((JPH_ShapeSettings*)settings);
            }
        }

        public float TopRadius
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TaperedCylinderShape_GetTopRadius((JPH_TaperedCylinderShape*)Handle); }
            }
        }

        public float BottomRadius
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TaperedCylinderShape_GetBottomRadius((JPH_TaperedCylinderShape*)Handle); }
            }
        }

        public float HalfHeight
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TaperedCylinderShape_GetHalfHeight((JPH_TaperedCylinderShape*)Handle); }
            }
        }

        public float ConvexRadius
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TaperedCylinderShape_GetConvexRadius((JPH_TaperedCylinderShape*)Handle); }
            }
        }
    }
}

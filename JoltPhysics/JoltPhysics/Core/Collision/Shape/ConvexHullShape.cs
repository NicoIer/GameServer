// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class ConvexHullShapeSettings : NativeHandle
    {
        public ConvexHullShapeSettings(Float3[] points, float maxConvexRadius = 0.05f)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                // Float3 is LayoutKind.Sequential (float x,y,z) — binary compatible with JPH_Vec3.
                fixed (Float3* pointsPtr = points)
                {
                    Handle = (IntPtr)Methods.JPH_ConvexHullShapeSettings_Create(
                        (JPH_Vec3*)pointsPtr, (uint)points.Length, maxConvexRadius);
                }
            }
        }

        public ConvexHullShape CreateShape()
        {
            ThrowIfDisposed();
            return new ConvexHullShape(this);
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_ShapeSettings_Destroy((JPH_ShapeSettings*)Handle);
            }
        }
    }

    public sealed class ConvexHullShape : Shape
    {
        internal ConvexHullShape(ConvexHullShapeSettings settings)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ConvexHullShapeSettings_CreateShape(
                    (JPH_ConvexHullShapeSettings*)settings.Handle);
            }
        }

        public uint NumPoints
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_ConvexHullShape_GetNumPoints(
                        (JPH_ConvexHullShape*)Handle);
                }
            }
        }

        public Float3 GetPoint(uint index)
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_Vec3 result;
                Methods.JPH_ConvexHullShape_GetPoint(
                    (JPH_ConvexHullShape*)Handle, index, &result);
                return new Float3(result.x, result.y, result.z);
            }
        }
    }
}

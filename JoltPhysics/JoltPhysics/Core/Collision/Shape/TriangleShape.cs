// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    public sealed class TriangleShape : Shape
    {
        public TriangleShape(Float3 v1, Float3 v2, Float3 v3, float convexRadius = 0f)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_TriangleShape_Create(
                    (JPH_Vec3*)&v1, (JPH_Vec3*)&v2, (JPH_Vec3*)&v3, convexRadius);
            }
        }

        public float ConvexRadius
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_TriangleShape_GetConvexRadius((JPH_TriangleShape*)Handle); }
            }
        }

        public Float3 Vertex1
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_TriangleShape_GetVertex1((JPH_TriangleShape*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }

        public Float3 Vertex2
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_TriangleShape_GetVertex2((JPH_TriangleShape*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }

        public Float3 Vertex3
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_TriangleShape_GetVertex3((JPH_TriangleShape*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }
    }
}

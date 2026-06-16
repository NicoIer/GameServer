// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class MeshShapeSettings : NativeHandle
    {
        public MeshShapeSettings(Float3[] vertices, int[] triangleIndices)
            : base(IntPtr.Zero, true)
        {
            if (triangleIndices.Length % 3 != 0)
                throw new ArgumentException("Triangle indices count must be a multiple of 3.");

            unsafe
            {
                int vertexCount = vertices.Length;
                int triangleCount = triangleIndices.Length / 3;

                // Float3 is LayoutKind.Sequential (float x,y,z) — binary compatible with JPH_Vec3.
                // Pin the managed array directly instead of stackalloc (safe for large meshes).
                var nativeTriangles = new JPH_IndexedTriangle[triangleCount];
                for (int i = 0; i < triangleCount; i++)
                {
                    nativeTriangles[i] = new JPH_IndexedTriangle
                    {
                        i1 = (uint)triangleIndices[i * 3],
                        i2 = (uint)triangleIndices[i * 3 + 1],
                        i3 = (uint)triangleIndices[i * 3 + 2],
                        materialIndex = 0,
                        userData = 0,
                    };
                }

                fixed (Float3* vertexPtr = vertices)
                fixed (JPH_IndexedTriangle* triPtr = nativeTriangles)
                {
                    Handle = (IntPtr)Methods.JPH_MeshShapeSettings_Create2(
                        (JPH_Vec3*)vertexPtr, (uint)vertexCount,
                        triPtr, (uint)triangleCount);
                }
            }
        }

        public MeshShape CreateShape()
        {
            ThrowIfDisposed();
            return new MeshShape(this);
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_ShapeSettings_Destroy((JPH_ShapeSettings*)Handle);
            }
        }
    }

    public sealed class MeshShape : Shape
    {
        internal MeshShape(MeshShapeSettings settings)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_MeshShapeSettings_CreateShape(
                    (JPH_MeshShapeSettings*)settings.Handle);
            }
        }
    }
}

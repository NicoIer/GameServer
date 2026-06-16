// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class HeightFieldShape : Shape
    {
        /// <summary>
        /// 通过 HeightFieldShapeSettings 创建。samples 数组长度应为 sampleCount * sampleCount。
        /// </summary>
        public HeightFieldShape(float[] samples, Float3 offset, Float3 scale, uint sampleCount)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                fixed (float* pSamples = samples)
                {
                    var nativeOffset = *(JPH_Vec3*)&offset;
                    var nativeScale = *(JPH_Vec3*)&scale;
                    var settings = Methods.JPH_HeightFieldShapeSettings_Create(
                        pSamples, &nativeOffset, &nativeScale, sampleCount, null);
                    Handle = (IntPtr)Methods.JPH_HeightFieldShapeSettings_CreateShape(settings);
                    Methods.JPH_ShapeSettings_Destroy((JPH_ShapeSettings*)settings);
                }
            }
        }

        internal HeightFieldShape(IntPtr handle) : base(handle) { }

        public uint SampleCount
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HeightFieldShape_GetSampleCount((JPH_HeightFieldShape*)Handle); }
            }
        }

        public uint BlockSize
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HeightFieldShape_GetBlockSize((JPH_HeightFieldShape*)Handle); }
            }
        }

        public float MinHeightValue
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HeightFieldShape_GetMinHeightValue((JPH_HeightFieldShape*)Handle); }
            }
        }

        public float MaxHeightValue
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HeightFieldShape_GetMaxHeightValue((JPH_HeightFieldShape*)Handle); }
            }
        }

        public Float3 GetPosition(uint x, uint y)
        {
            ThrowIfDisposed();
            unsafe
            {
                Float3 result;
                Methods.JPH_HeightFieldShape_GetPosition((JPH_HeightFieldShape*)Handle, x, y, (JPH_Vec3*)&result);
                return result;
            }
        }

        public bool IsNoCollision(uint x, uint y)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_HeightFieldShape_IsNoCollision((JPH_HeightFieldShape*)Handle, x, y) != 0; }
        }

        public bool ProjectOntoSurface(Float3 localPosition, out Float3 surfacePosition, out uint subShapeID)
        {
            ThrowIfDisposed();
            unsafe
            {
                Float3 outPos;
                uint outID;
                bool result = Methods.JPH_HeightFieldShape_ProjectOntoSurface(
                    (JPH_HeightFieldShape*)Handle, (JPH_Vec3*)&localPosition,
                    (JPH_Vec3*)&outPos, &outID) != 0;
                surfacePosition = outPos;
                subShapeID = outID;
                return result;
            }
        }
    }
}

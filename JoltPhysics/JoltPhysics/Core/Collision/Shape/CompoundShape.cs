// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    #region CompoundShape base

    public abstract class CompoundShape : Shape
    {
        internal CompoundShape(IntPtr handle) : base(handle) { }
        internal CompoundShape(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle) { }

        public uint NumSubShapes
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_CompoundShape_GetNumSubShapes((JPH_CompoundShape*)Handle); }
            }
        }

        public uint GetSubShapeIndexFromID(uint subShapeID, out uint remainder)
        {
            ThrowIfDisposed();
            unsafe
            {
                uint rem;
                uint index = Methods.JPH_CompoundShape_GetSubShapeIndexFromID(
                    (JPH_CompoundShape*)Handle, subShapeID, &rem);
                remainder = rem;
                return index;
            }
        }
    }

    #endregion

    #region StaticCompoundShape

    public sealed class StaticCompoundShapeSettings : NativeHandle
    {
        public StaticCompoundShapeSettings() : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_StaticCompoundShapeSettings_Create();
            }
        }

        public void AddShape(Float3 position, Quat rotation, Shape shape, uint userData = 0)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_CompoundShapeSettings_AddShape2(
                    (JPH_CompoundShapeSettings*)Handle,
                    (JPH_Vec3*)&position, (JPH_Quat*)&rotation,
                    (JPH_Shape*)shape.Handle, userData);
            }
        }

        public StaticCompoundShape CreateShape()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = Methods.JPH_StaticCompoundShape_Create(
                    (JPH_StaticCompoundShapeSettings*)Handle);
                return new StaticCompoundShape((IntPtr)ptr);
            }
        }

        protected override void DestroyNative()
        {
            unsafe { Methods.JPH_ShapeSettings_Destroy((JPH_ShapeSettings*)Handle); }
        }
    }

    public sealed class StaticCompoundShape : CompoundShape
    {
        internal StaticCompoundShape(IntPtr handle) : base(handle) { }
    }

    #endregion

    #region MutableCompoundShape

    public sealed class MutableCompoundShapeSettings : NativeHandle
    {
        public MutableCompoundShapeSettings() : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_MutableCompoundShapeSettings_Create();
            }
        }

        public void AddShape(Float3 position, Quat rotation, Shape shape, uint userData = 0)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_CompoundShapeSettings_AddShape2(
                    (JPH_CompoundShapeSettings*)Handle,
                    (JPH_Vec3*)&position, (JPH_Quat*)&rotation,
                    (JPH_Shape*)shape.Handle, userData);
            }
        }

        public MutableCompoundShape CreateShape()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = Methods.JPH_MutableCompoundShape_Create(
                    (JPH_MutableCompoundShapeSettings*)Handle);
                return new MutableCompoundShape((IntPtr)ptr);
            }
        }

        protected override void DestroyNative()
        {
            unsafe { Methods.JPH_ShapeSettings_Destroy((JPH_ShapeSettings*)Handle); }
        }
    }

    public sealed class MutableCompoundShape : CompoundShape
    {
        internal MutableCompoundShape(IntPtr handle) : base(handle) { }

        public uint AddShape(Float3 position, Quat rotation, Shape child, uint userData = 0, uint index = uint.MaxValue)
        {
            ThrowIfDisposed();
            unsafe
            {
                return Methods.JPH_MutableCompoundShape_AddShape(
                    (JPH_MutableCompoundShape*)Handle,
                    (JPH_Vec3*)&position, (JPH_Quat*)&rotation,
                    (JPH_Shape*)child.Handle, userData, index);
            }
        }

        public void RemoveShape(uint index)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_MutableCompoundShape_RemoveShape((JPH_MutableCompoundShape*)Handle, index); }
        }

        public void ModifyShape(uint index, Float3 position, Quat rotation)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_MutableCompoundShape_ModifyShape(
                    (JPH_MutableCompoundShape*)Handle, index,
                    (JPH_Vec3*)&position, (JPH_Quat*)&rotation);
            }
        }

        public void ModifyShape(uint index, Float3 position, Quat rotation, Shape newShape)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_MutableCompoundShape_ModifyShape2(
                    (JPH_MutableCompoundShape*)Handle, index,
                    (JPH_Vec3*)&position, (JPH_Quat*)&rotation,
                    (JPH_Shape*)newShape.Handle);
            }
        }

        public void AdjustCenterOfMass()
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_MutableCompoundShape_AdjustCenterOfMass((JPH_MutableCompoundShape*)Handle); }
        }
    }

    #endregion
}

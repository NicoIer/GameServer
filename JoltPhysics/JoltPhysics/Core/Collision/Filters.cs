// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.InteropServices;
#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace JoltPhysics
{
    /// <summary>
    /// Filter for broadphase layer during queries.
    /// Subclass and override <see cref="ShouldCollide"/> to implement custom filtering.
    /// </summary>
    public abstract class BroadPhaseLayerFilter : NativeHandle
    {
        private static readonly JPH_BroadPhaseLayerFilter_Procs s_procs;
        private GCHandle _gcHandle;

#if !NET5_0_OR_GREATER
        private unsafe delegate byte ShouldCollideNative(void* userData, byte layer);
        private static readonly ShouldCollideNative s_shouldCollideDel;
#endif

        static unsafe BroadPhaseLayerFilter()
        {
#if NET5_0_OR_GREATER
            s_procs = new JPH_BroadPhaseLayerFilter_Procs
            {
                ShouldCollide = &ShouldCollideCallback,
            };
#else
            s_shouldCollideDel = ShouldCollideCallback;
            s_procs = new JPH_BroadPhaseLayerFilter_Procs
            {
                ShouldCollide = (delegate* unmanaged[Cdecl]<void*, byte, byte>)
                    Marshal.GetFunctionPointerForDelegate(s_shouldCollideDel),
            };
#endif
            fixed (JPH_BroadPhaseLayerFilter_Procs* ptr = &s_procs)
            {
                Methods.JPH_BroadPhaseLayerFilter_SetProcs(ptr);
            }
        }

        protected BroadPhaseLayerFilter() : base(IntPtr.Zero, true)
        {
            _gcHandle = GCHandle.Alloc(this);
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_BroadPhaseLayerFilter_Create(
                    (void*)GCHandle.ToIntPtr(_gcHandle));
            }
        }

        protected abstract bool ShouldCollide(BroadPhaseLayer layer);

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe byte ShouldCollideCallback(void* userData, byte layer)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is BroadPhaseLayerFilter filter)
                return (byte)(filter.ShouldCollide(new BroadPhaseLayer(layer)) ? 1 : 0);
            return 1;
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_BroadPhaseLayerFilter_Destroy((JPH_BroadPhaseLayerFilter*)Handle);
            }
            if (_gcHandle.IsAllocated)
                _gcHandle.Free();
        }
    }

    /// <summary>
    /// Filter for object layer during queries.
    /// Subclass and override <see cref="ShouldCollide"/> to implement custom filtering.
    /// </summary>
    public abstract class ObjectLayerFilter : NativeHandle
    {
        private static readonly JPH_ObjectLayerFilter_Procs s_procs;
        private GCHandle _gcHandle;

#if !NET5_0_OR_GREATER
        private unsafe delegate byte ShouldCollideNative(void* userData, uint layer);
        private static readonly ShouldCollideNative s_shouldCollideDel;
#endif

        static unsafe ObjectLayerFilter()
        {
#if NET5_0_OR_GREATER
            s_procs = new JPH_ObjectLayerFilter_Procs
            {
                ShouldCollide = &ShouldCollideCallback,
            };
#else
            s_shouldCollideDel = ShouldCollideCallback;
            s_procs = new JPH_ObjectLayerFilter_Procs
            {
                ShouldCollide = (delegate* unmanaged[Cdecl]<void*, uint, byte>)
                    Marshal.GetFunctionPointerForDelegate(s_shouldCollideDel),
            };
#endif
            fixed (JPH_ObjectLayerFilter_Procs* ptr = &s_procs)
            {
                Methods.JPH_ObjectLayerFilter_SetProcs(ptr);
            }
        }

        protected ObjectLayerFilter() : base(IntPtr.Zero, true)
        {
            _gcHandle = GCHandle.Alloc(this);
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ObjectLayerFilter_Create(
                    (void*)GCHandle.ToIntPtr(_gcHandle));
            }
        }

        protected abstract bool ShouldCollide(ObjectLayer layer);

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe byte ShouldCollideCallback(void* userData, uint layer)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is ObjectLayerFilter filter)
                return (byte)(filter.ShouldCollide(new ObjectLayer(layer)) ? 1 : 0);
            return 1;
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_ObjectLayerFilter_Destroy((JPH_ObjectLayerFilter*)Handle);
            }
            if (_gcHandle.IsAllocated)
                _gcHandle.Free();
        }
    }

    /// <summary>
    /// Filter for bodies during queries.
    /// Subclass and override <see cref="ShouldCollide"/> and <see cref="ShouldCollideLocked"/> to implement custom filtering.
    /// </summary>
    public abstract class BodyFilter : NativeHandle
    {
        private static readonly JPH_BodyFilter_Procs s_procs;
        private GCHandle _gcHandle;

#if !NET5_0_OR_GREATER
        private unsafe delegate byte ShouldCollideNative(void* userData, uint bodyID);
        private unsafe delegate byte ShouldCollideLockedNative(void* userData, JPH_Body* body);
        private static readonly ShouldCollideNative s_shouldCollideDel;
        private static readonly ShouldCollideLockedNative s_shouldCollideLockedDel;
#endif

        static unsafe BodyFilter()
        {
#if NET5_0_OR_GREATER
            s_procs = new JPH_BodyFilter_Procs
            {
                ShouldCollide = &ShouldCollideCallback,
                ShouldCollideLocked = &ShouldCollideLockedCallback,
            };
#else
            s_shouldCollideDel = ShouldCollideCallback;
            s_shouldCollideLockedDel = ShouldCollideLockedCallback;
            s_procs = new JPH_BodyFilter_Procs
            {
                ShouldCollide = (delegate* unmanaged[Cdecl]<void*, uint, byte>)
                    Marshal.GetFunctionPointerForDelegate(s_shouldCollideDel),
                ShouldCollideLocked = (delegate* unmanaged[Cdecl]<void*, JPH_Body*, byte>)
                    Marshal.GetFunctionPointerForDelegate(s_shouldCollideLockedDel),
            };
#endif
            fixed (JPH_BodyFilter_Procs* ptr = &s_procs)
            {
                Methods.JPH_BodyFilter_SetProcs(ptr);
            }
        }

        protected BodyFilter() : base(IntPtr.Zero, true)
        {
            _gcHandle = GCHandle.Alloc(this);
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_BodyFilter_Create(
                    (void*)GCHandle.ToIntPtr(_gcHandle));
            }
        }

        protected abstract bool ShouldCollide(BodyID bodyID);
        protected abstract bool ShouldCollideLocked(BodyID bodyID);

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe byte ShouldCollideCallback(void* userData, uint bodyID)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is BodyFilter filter)
                return (byte)(filter.ShouldCollide(new BodyID(bodyID)) ? 1 : 0);
            return 1;
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe byte ShouldCollideLockedCallback(void* userData, JPH_Body* body)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is BodyFilter filter)
            {
                uint id = Methods.JPH_Body_GetID(body);
                return (byte)(filter.ShouldCollideLocked(new BodyID(id)) ? 1 : 0);
            }
            return 1;
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_BodyFilter_Destroy((JPH_BodyFilter*)Handle);
            }
            if (_gcHandle.IsAllocated)
                _gcHandle.Free();
        }
    }

    /// <summary>
    /// Filter for shapes during queries.
    /// Subclass and override <see cref="ShouldCollide(Shape, uint)"/> to implement custom filtering.
    /// </summary>
    public abstract class ShapeFilter : NativeHandle
    {
        private static readonly JPH_ShapeFilter_Procs s_procs;
        private GCHandle _gcHandle;

#if !NET5_0_OR_GREATER
        private unsafe delegate byte ShouldCollideNative(void* userData, JPH_Shape* shape, uint* subShapeID);
        private unsafe delegate byte ShouldCollide2Native(void* userData, JPH_Shape* shape1, uint* subShapeID1, JPH_Shape* shape2, uint* subShapeID2);
        private static readonly ShouldCollideNative s_shouldCollideDel;
        private static readonly ShouldCollide2Native s_shouldCollide2Del;
#endif

        static unsafe ShapeFilter()
        {
#if NET5_0_OR_GREATER
            s_procs = new JPH_ShapeFilter_Procs
            {
                ShouldCollide = &ShouldCollideCallback,
                ShouldCollide2 = &ShouldCollide2Callback,
            };
#else
            s_shouldCollideDel = ShouldCollideCallback;
            s_shouldCollide2Del = ShouldCollide2Callback;
            s_procs = new JPH_ShapeFilter_Procs
            {
                ShouldCollide = (delegate* unmanaged[Cdecl]<void*, JPH_Shape*, uint*, byte>)
                    Marshal.GetFunctionPointerForDelegate(s_shouldCollideDel),
                ShouldCollide2 = (delegate* unmanaged[Cdecl]<void*, JPH_Shape*, uint*, JPH_Shape*, uint*, byte>)
                    Marshal.GetFunctionPointerForDelegate(s_shouldCollide2Del),
            };
#endif
            fixed (JPH_ShapeFilter_Procs* ptr = &s_procs)
            {
                Methods.JPH_ShapeFilter_SetProcs(ptr);
            }
        }

        protected ShapeFilter() : base(IntPtr.Zero, true)
        {
            _gcHandle = GCHandle.Alloc(this);
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ShapeFilter_Create(
                    (void*)GCHandle.ToIntPtr(_gcHandle));
            }
        }

        public BodyID BodyID2
        {
            get
            {
                unsafe
                {
                    return new BodyID(Methods.JPH_ShapeFilter_GetBodyID2((JPH_ShapeFilter*)Handle));
                }
            }
            set
            {
                unsafe
                {
                    Methods.JPH_ShapeFilter_SetBodyID2((JPH_ShapeFilter*)Handle, value.Value);
                }
            }
        }

        protected virtual bool ShouldCollide(Shape shape2, uint subShapeID2)
        {
            return true;
        }

        protected virtual bool ShouldCollide(Shape shape1, uint subShapeID1, Shape shape2, uint subShapeID2)
        {
            return true;
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe byte ShouldCollideCallback(void* userData, JPH_Shape* shape, uint* subShapeID)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is ShapeFilter filter)
                return (byte)(filter.ShouldCollide(new ShapeHandle((IntPtr)shape), *subShapeID) ? 1 : 0);
            return 1;
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe byte ShouldCollide2Callback(void* userData,
            JPH_Shape* shape1, uint* subShapeID1, JPH_Shape* shape2, uint* subShapeID2)
        {
            var handle = GCHandle.FromIntPtr((IntPtr)userData);
            if (handle.Target is ShapeFilter filter)
                return (byte)(filter.ShouldCollide(
                    new ShapeHandle((IntPtr)shape1), *subShapeID1,
                    new ShapeHandle((IntPtr)shape2), *subShapeID2) ? 1 : 0);
            return 1;
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_ShapeFilter_Destroy((JPH_ShapeFilter*)Handle);
            }
            if (_gcHandle.IsAllocated)
                _gcHandle.Free();
        }

        /// <summary>
        /// Lightweight non-owning Shape handle for use in filter callbacks.
        /// </summary>
        internal sealed class ShapeHandle : Shape
        {
            internal ShapeHandle(IntPtr handle) : base(handle, false) { }
            protected override void DestroyNative() { }
        }
    }
}

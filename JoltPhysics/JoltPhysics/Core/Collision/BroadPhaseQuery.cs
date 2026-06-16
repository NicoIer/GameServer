// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JoltPhysics
{
    public struct BroadPhaseCastResult
    {
        public BodyID BodyID;
        public float Fraction;
    }

    public readonly struct BroadPhaseQuery
    {
        internal readonly IntPtr Handle;

        internal BroadPhaseQuery(IntPtr handle)
        {
            Handle = handle;
        }

        private unsafe JPH_BroadPhaseQuery* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (JPH_BroadPhaseQuery*)Handle;
        }

        #region Callbacks

#if !NET5_0_OR_GREATER
        private unsafe delegate float RayCastBodyCollectorNative(void* userData, JPH_BroadPhaseCastResult* result);
        private unsafe delegate float CollideShapeBodyCollectorNative(void* userData, uint bodyID);

        private static readonly RayCastBodyCollectorNative s_rayCastBodyDel = OnRayCastBodyCollectorCallback;
        private static readonly CollideShapeBodyCollectorNative s_collideShapeBodyDel = OnCollideShapeBodyCollectorCallback;

        private static readonly IntPtr s_pRayCastBody = Marshal.GetFunctionPointerForDelegate(s_rayCastBodyDel);
        private static readonly IntPtr s_pCollideShapeBody = Marshal.GetFunctionPointerForDelegate(s_collideShapeBodyDel);
#endif

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe float OnRayCastBodyCollectorCallback(void* userData, JPH_BroadPhaseCastResult* result)
        {
            var list = (ICollection<BroadPhaseCastResult>)GCHandle.FromIntPtr((IntPtr)userData).Target!;
            list.Add(new BroadPhaseCastResult
            {
                BodyID = new BodyID(result->bodyID),
                Fraction = result->fraction,
            });
            // Return 1.0 to continue collecting all hits
            return 1.0f;
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe float OnCollideShapeBodyCollectorCallback(void* userData, uint bodyID)
        {
            var list = (ICollection<BodyID>)GCHandle.FromIntPtr((IntPtr)userData).Target!;
            list.Add(new BodyID(bodyID));
            // Return 1.0 to continue collecting all hits
            return 1.0f;
        }

        #endregion

        #region CastRay

        /// <summary>
        /// BroadPhase 射线检测，收集所有命中的 body。
        /// direction 的长度即为射线最大距离。
        /// </summary>
        public bool CastRay(Float3 origin, Float3 direction,
            ICollection<BroadPhaseCastResult> results,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var o = *(JPH_Vec3*)&origin;
                    var d = *(JPH_Vec3*)&direction;
                    bool ret = Methods.JPH_BroadPhaseQuery_CastRay(
                        Ptr, &o, &d,
#if NET5_0_OR_GREATER
                        &OnRayCastBodyCollectorCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, JPH_BroadPhaseCastResult*, float>)s_pRayCastBody,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero)) != 0;
                    return ret;
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        #endregion

        #region CollideAABox

        /// <summary>
        /// BroadPhase AABB 碰撞检测，收集所有重叠的 body ID。
        /// </summary>
        public bool CollideAABox(Float3 min, Float3 max,
            ICollection<BodyID> results,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var box = new JPH_AABox
                    {
                        min = *(JPH_Vec3*)&min,
                        max = *(JPH_Vec3*)&max,
                    };
                    bool ret = Methods.JPH_BroadPhaseQuery_CollideAABox(
                        Ptr, &box,
#if NET5_0_OR_GREATER
                        &OnCollideShapeBodyCollectorCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, uint, float>)s_pCollideShapeBody,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero)) != 0;
                    return ret;
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        #endregion

        #region CollideSphere

        /// <summary>
        /// BroadPhase 球体碰撞检测，收集所有重叠的 body ID。
        /// </summary>
        public bool CollideSphere(Float3 center, float radius,
            ICollection<BodyID> results,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var c = *(JPH_Vec3*)&center;
                    bool ret = Methods.JPH_BroadPhaseQuery_CollideSphere(
                        Ptr, &c, radius,
#if NET5_0_OR_GREATER
                        &OnCollideShapeBodyCollectorCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, uint, float>)s_pCollideShapeBody,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero)) != 0;
                    return ret;
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        #endregion

        #region CollidePoint

        /// <summary>
        /// BroadPhase 点碰撞检测，收集所有包含该点的 body ID。
        /// </summary>
        public bool CollidePoint(Float3 point,
            ICollection<BodyID> results,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var p = *(JPH_Vec3*)&point;
                    bool ret = Methods.JPH_BroadPhaseQuery_CollidePoint(
                        Ptr, &p,
#if NET5_0_OR_GREATER
                        &OnCollideShapeBodyCollectorCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, uint, float>)s_pCollideShapeBody,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero)) != 0;
                    return ret;
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        #endregion
    }
}

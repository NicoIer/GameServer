// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JoltPhysics
{
    #region Result Structs

    public struct RayCastResult
    {
        const float CEpsilon = 1.192092896e-07F;

        public BodyID BodyID;
        public float Fraction;
        public uint SubShapeID2;

        public static RayCastResult Default => new RayCastResult
        {
            BodyID = BodyID.Invalid,
            Fraction = 1.0f + CEpsilon,
        };
    }

    public struct CollidePointResult
    {
        public BodyID BodyID;
        public uint SubShapeID2;
    }

    public struct CollideShapeResult
    {
        public Float3 ContactPointOn1;
        public Float3 ContactPointOn2;
        public Float3 PenetrationAxis;
        public float PenetrationDepth;
        public uint SubShapeID1;
        public uint SubShapeID2;
        public BodyID BodyID2;
    }

    public struct ShapeCastResult
    {
        public Float3 ContactPointOn1;
        public Float3 ContactPointOn2;
        public Float3 PenetrationAxis;
        public float PenetrationDepth;
        public uint SubShapeID1;
        public uint SubShapeID2;
        public BodyID BodyID2;
        public float Fraction;
        public bool IsBackFaceHit;
    }

    public delegate float CastShapeCollectorCallback(in ShapeCastResult result);

    #endregion

    #region Settings Structs

    public struct RayCastSettings
    {
        public BackFaceMode BackFaceModeTriangles;
        public BackFaceMode BackFaceModeConvex;
        public bool TreatConvexAsSolid;

        public static RayCastSettings Default => new RayCastSettings
        {
            BackFaceModeTriangles = BackFaceMode.IgnoreBackFaces,
            BackFaceModeConvex = BackFaceMode.IgnoreBackFaces,
            TreatConvexAsSolid = true,
        };

        internal readonly JPH_RayCastSettings ToNative() => new JPH_RayCastSettings
        {
            backFaceModeTriangles = (JPH_BackFaceMode)BackFaceModeTriangles,
            backFaceModeConvex = (JPH_BackFaceMode)BackFaceModeConvex,
            treatConvexAsSolid = (byte)(TreatConvexAsSolid ? 1 : 0),
        };
    }

    public struct CollideShapeSettings
    {
        public ActiveEdgeMode ActiveEdgeMode;
        public CollectFacesMode CollectFacesMode;
        public float CollisionTolerance;
        public float PenetrationTolerance;
        public Float3 ActiveEdgeMovementDirection;
        public float MaxSeparationDistance;
        public BackFaceMode BackFaceMode;

        public static CollideShapeSettings Default
        {
            get
            {
                unsafe
                {
                    JPH_CollideShapeSettings native;
                    Methods.JPH_CollideShapeSettings_Init(&native);
                    return new CollideShapeSettings
                    {
                        ActiveEdgeMode = (ActiveEdgeMode)native.@base.activeEdgeMode,
                        CollectFacesMode = (CollectFacesMode)native.@base.collectFacesMode,
                        CollisionTolerance = native.@base.collisionTolerance,
                        PenetrationTolerance = native.@base.penetrationTolerance,
                        ActiveEdgeMovementDirection = new Float3(
                            native.@base.activeEdgeMovementDirection.x,
                            native.@base.activeEdgeMovementDirection.y,
                            native.@base.activeEdgeMovementDirection.z),
                        MaxSeparationDistance = native.maxSeparationDistance,
                        BackFaceMode = (BackFaceMode)native.backFaceMode,
                    };
                }
            }
        }

        internal readonly unsafe void ToNative(JPH_CollideShapeSettings* native)
        {
            Methods.JPH_CollideShapeSettings_Init(native);

            if (IsDefault)
                return;

            native->@base.activeEdgeMode = (JPH_ActiveEdgeMode)ActiveEdgeMode;
            native->@base.collectFacesMode = (JPH_CollectFacesMode)CollectFacesMode;
            native->@base.collisionTolerance = CollisionTolerance;
            native->@base.penetrationTolerance = PenetrationTolerance;
            var dir = ActiveEdgeMovementDirection;
            native->@base.activeEdgeMovementDirection = *(JPH_Vec3*)&dir;
            native->maxSeparationDistance = MaxSeparationDistance;
            native->backFaceMode = (JPH_BackFaceMode)BackFaceMode;
        }

        private readonly bool IsDefault =>
            ActiveEdgeMode == default &&
            CollectFacesMode == default &&
            CollisionTolerance == default &&
            PenetrationTolerance == default &&
            ActiveEdgeMovementDirection == default &&
            MaxSeparationDistance == default &&
            BackFaceMode == default;
    }

    public struct ShapeCastSettings
    {
        public ActiveEdgeMode ActiveEdgeMode;
        public CollectFacesMode CollectFacesMode;
        public float CollisionTolerance;
        public float PenetrationTolerance;
        public Float3 ActiveEdgeMovementDirection;
        public BackFaceMode BackFaceModeTriangles;
        public BackFaceMode BackFaceModeConvex;
        public bool UseShrunkenShapeAndConvexRadius;
        public bool ReturnDeepestPoint;

        public static ShapeCastSettings Default
        {
            get
            {
                unsafe
                {
                    JPH_ShapeCastSettings native;
                    Methods.JPH_ShapeCastSettings_Init(&native);
                    return new ShapeCastSettings
                    {
                        ActiveEdgeMode = (ActiveEdgeMode)native.@base.activeEdgeMode,
                        CollectFacesMode = (CollectFacesMode)native.@base.collectFacesMode,
                        CollisionTolerance = native.@base.collisionTolerance,
                        PenetrationTolerance = native.@base.penetrationTolerance,
                        ActiveEdgeMovementDirection = new Float3(
                            native.@base.activeEdgeMovementDirection.x,
                            native.@base.activeEdgeMovementDirection.y,
                            native.@base.activeEdgeMovementDirection.z),
                        BackFaceModeTriangles = (BackFaceMode)native.backFaceModeTriangles,
                        BackFaceModeConvex = (BackFaceMode)native.backFaceModeConvex,
                        UseShrunkenShapeAndConvexRadius = native.useShrunkenShapeAndConvexRadius != 0,
                        ReturnDeepestPoint = native.returnDeepestPoint != 0,
                    };
                }
            }
        }

        internal readonly unsafe void ToNative(JPH_ShapeCastSettings* native)
        {
            Methods.JPH_ShapeCastSettings_Init(native);
            native->@base.activeEdgeMode = (JPH_ActiveEdgeMode)ActiveEdgeMode;
            native->@base.collectFacesMode = (JPH_CollectFacesMode)CollectFacesMode;
            native->@base.collisionTolerance = CollisionTolerance;
            native->@base.penetrationTolerance = PenetrationTolerance;
            var dir = ActiveEdgeMovementDirection;
            native->@base.activeEdgeMovementDirection = *(JPH_Vec3*)&dir;
            native->backFaceModeTriangles = (JPH_BackFaceMode)BackFaceModeTriangles;
            native->backFaceModeConvex = (JPH_BackFaceMode)BackFaceModeConvex;
            native->useShrunkenShapeAndConvexRadius = (byte)(UseShrunkenShapeAndConvexRadius ? 1 : 0);
            native->returnDeepestPoint = (byte)(ReturnDeepestPoint ? 1 : 0);
        }
    }

    #endregion

    public readonly struct NarrowPhaseQuery
    {
        internal readonly IntPtr Handle;

        internal NarrowPhaseQuery(IntPtr handle)
        {
            Handle = handle;
        }

        private unsafe JPH_NarrowPhaseQuery* Ptr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (JPH_NarrowPhaseQuery*)Handle;
        }


        #region Callbacks

#if !NET5_0_OR_GREATER
        private unsafe delegate void CastRayResultNative(void* userData, JPH_RayCastResult* result);
        private unsafe delegate void CollidePointResultNative(void* userData, JPH_CollidePointResult* result);
        private unsafe delegate void CollideShapeResultNative(void* userData, JPH_CollideShapeResult* result);
        private unsafe delegate void CastShapeResultNative(void* userData, JPH_ShapeCastResult* result);
        private unsafe delegate float CastShapeCollectorNative(void* userData, JPH_ShapeCastResult* result);

        private static readonly CastRayResultNative s_castRayDel = OnCastRayResultCallback;
        private static readonly CollidePointResultNative s_collidePointDel = OnCollidePointCallback;
        private static readonly CollideShapeResultNative s_collideShapeDel = OnCollideShapeCallback;
        private static readonly CastShapeResultNative s_castShapeDel = OnCastShapeResultCallback;
        private static readonly CastShapeCollectorNative s_castShapeCollectorDel = OnCastShapeCollectorCallback;

        private static readonly IntPtr s_pCastRay = Marshal.GetFunctionPointerForDelegate(s_castRayDel);
        private static readonly IntPtr s_pCollidePoint = Marshal.GetFunctionPointerForDelegate(s_collidePointDel);
        private static readonly IntPtr s_pCollideShape = Marshal.GetFunctionPointerForDelegate(s_collideShapeDel);
        private static readonly IntPtr s_pCastShape = Marshal.GetFunctionPointerForDelegate(s_castShapeDel);
        private static readonly IntPtr s_pCastShapeCollector = Marshal.GetFunctionPointerForDelegate(s_castShapeCollectorDel);
#endif

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe void OnCastRayResultCallback(void* userData, JPH_RayCastResult* result)
        {
            var list = (ICollection<RayCastResult>)GCHandle.FromIntPtr((IntPtr)userData).Target!;
            list.Add(new RayCastResult
            {
                BodyID = new BodyID(result->bodyID),
                Fraction = result->fraction,
                SubShapeID2 = result->subShapeID2,
            });
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe void OnCollidePointCallback(void* userData, JPH_CollidePointResult* result)
        {
            var list = (ICollection<CollidePointResult>)GCHandle.FromIntPtr((IntPtr)userData).Target!;
            list.Add(new CollidePointResult
            {
                BodyID = new BodyID(result->bodyID),
                SubShapeID2 = result->subShapeID2,
            });
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe void OnCollideShapeCallback(void* userData, JPH_CollideShapeResult* result)
        {
            var list = (ICollection<CollideShapeResult>)GCHandle.FromIntPtr((IntPtr)userData).Target!;
            list.Add(new CollideShapeResult
            {
                ContactPointOn1 = *(Float3*)&result->contactPointOn1,
                ContactPointOn2 = *(Float3*)&result->contactPointOn2,
                PenetrationAxis = *(Float3*)&result->penetrationAxis,
                PenetrationDepth = result->penetrationDepth,
                SubShapeID1 = result->subShapeID1,
                SubShapeID2 = result->subShapeID2,
                BodyID2 = new BodyID(result->bodyID2),
            });
        }

        private static unsafe ShapeCastResult FromNativeShapeCastResult(JPH_ShapeCastResult* result) => new ShapeCastResult
        {
            ContactPointOn1 = *(Float3*)&result->contactPointOn1,
            ContactPointOn2 = *(Float3*)&result->contactPointOn2,
            PenetrationAxis = *(Float3*)&result->penetrationAxis,
            PenetrationDepth = result->penetrationDepth,
            SubShapeID1 = result->subShapeID1,
            SubShapeID2 = result->subShapeID2,
            BodyID2 = new BodyID(result->bodyID2),
            Fraction = result->fraction,
            IsBackFaceHit = result->isBackFaceHit != 0,
        };

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe void OnCastShapeResultCallback(void* userData, JPH_ShapeCastResult* result)
        {
            var list = (ICollection<ShapeCastResult>)GCHandle.FromIntPtr((IntPtr)userData).Target!;
            list.Add(FromNativeShapeCastResult(result));
        }

#if NET5_0_OR_GREATER
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
#endif
        private static unsafe float OnCastShapeCollectorCallback(void* userData, JPH_ShapeCastResult* result)
        {
            var callback = (CastShapeCollectorCallback)GCHandle.FromIntPtr((IntPtr)userData).Target!;
            var managedResult = FromNativeShapeCastResult(result);
            return callback(in managedResult);
        }

        #endregion

        #region CastRay

        /// <summary>
        /// Cast a ray, returns the closest hit.
        /// direction length = max ray distance (non-normalized).
        /// fraction ∈ [0,1], actual distance = fraction * |direction|.
        /// </summary>
        public bool CastRay(Float3 origin, Float3 direction, out RayCastResult hit,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null,
            BodyFilter? bodyFilter = null)
        {
            hit = default;
            unsafe
            {
                var o = *(JPH_Vec3*)&origin;
                var d = *(JPH_Vec3*)&direction;
                JPH_RayCastResult nativeHit;
                if (Methods.JPH_NarrowPhaseQuery_CastRay(
                        Ptr, &o, &d, &nativeHit,
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_BodyFilter*)(bodyFilter?.Handle ?? IntPtr.Zero)) != 0)
                {
                    hit = new RayCastResult
                    {
                        BodyID = new BodyID(nativeHit.bodyID),
                        Fraction = nativeHit.fraction,
                        SubShapeID2 = nativeHit.subShapeID2,
                    };
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Cast a ray, collect all results using the specified collector type.
        /// </summary>
        public bool CastRay(Float3 origin, Float3 direction,
            RayCastSettings settings,
            CollisionCollectorType collectorType,
            ICollection<RayCastResult> results,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null,
            BodyFilter? bodyFilter = null,
            ShapeFilter? shapeFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var o = *(JPH_Vec3*)&origin;
                    var d = *(JPH_Vec3*)&direction;
                    var native = settings.ToNative();
                    bool ret = Methods.JPH_NarrowPhaseQuery_CastRay3(
                        Ptr, &o, &d, &native,
                        (JPH_CollisionCollectorType)collectorType,
#if NET5_0_OR_GREATER
                        &OnCastRayResultCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, JPH_RayCastResult*, void>)s_pCastRay,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_BodyFilter*)(bodyFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ShapeFilter*)(shapeFilter?.Handle ?? IntPtr.Zero)) != 0;
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
        /// Check if a point is inside any body, collect all results.
        /// </summary>
        public bool CollidePoint(Float3 point,
            CollisionCollectorType collectorType,
            ICollection<CollidePointResult> results,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null,
            BodyFilter? bodyFilter = null,
            ShapeFilter? shapeFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var p = *(JPH_Vec3*)&point;
                    bool ret = Methods.JPH_NarrowPhaseQuery_CollidePoint2(
                        Ptr, &p,
                        (JPH_CollisionCollectorType)collectorType,
#if NET5_0_OR_GREATER
                        &OnCollidePointCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, JPH_CollidePointResult*, void>)s_pCollidePoint,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_BodyFilter*)(bodyFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ShapeFilter*)(shapeFilter?.Handle ?? IntPtr.Zero)) != 0;
                    return ret;
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        #endregion

        #region CollideShape

        public bool CollideShape(Shape shape,
            Float3 scale,
            Mat4 transform,
            Float3 baseOffset,
            ICollection<CollideShapeResult> results,
            in CollideShapeSettings settings,
            CollisionCollectorType collectorType = CollisionCollectorType.AllHit,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null,
            BodyFilter? bodyFilter = null,
            ShapeFilter? shapeFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var s = *(JPH_Vec3*)&scale;
                    var bo = *(JPH_Vec3*)&baseOffset;
                    var m = *(JPH_Mat4*)&transform;
                    JPH_CollideShapeSettings native;
                    settings.ToNative(&native);
                    bool ret = Methods.JPH_NarrowPhaseQuery_CollideShape2(
                        Ptr, (JPH_Shape*)shape.Handle, &s, &m,
                        &native,
                        &bo,
                        (JPH_CollisionCollectorType)collectorType,
#if NET5_0_OR_GREATER
                        &OnCollideShapeCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, JPH_CollideShapeResult*, void>)s_pCollideShape,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_BodyFilter*)(bodyFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ShapeFilter*)(shapeFilter?.Handle ?? IntPtr.Zero)) != 0;
                    return ret;
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        #endregion

        #region CastShape

        /// <summary>
        /// Cast a shape along direction using the low-level native CastShape collector callback.
        /// The callback return value is forwarded to Jolt as the early-out fraction.
        /// </summary>
        public bool CastShape(Shape shape,
            Mat4 transform,
            Float3 direction,
            in ShapeCastSettings settings,
            Float3 baseOffset,
            CastShapeCollectorCallback callback,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null,
            BodyFilter? bodyFilter = null,
            ShapeFilter? shapeFilter = null)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var gcHandle = GCHandle.Alloc(callback);
            try
            {
                unsafe
                {
                    var m = *(JPH_Mat4*)&transform;
                    var d = *(JPH_Vec3*)&direction;
                    JPH_ShapeCastSettings native;
                    settings.ToNative(&native);
                    var bo = *(JPH_Vec3*)&baseOffset;
                    bool ret = Methods.JPH_NarrowPhaseQuery_CastShape(
                        Ptr, (JPH_Shape*)shape.Handle, &m, &d,
                        &native,
                        &bo,
#if NET5_0_OR_GREATER
                        &OnCastShapeCollectorCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, JPH_ShapeCastResult*, float>)s_pCastShapeCollector,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_BodyFilter*)(bodyFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ShapeFilter*)(shapeFilter?.Handle ?? IntPtr.Zero)) != 0;
                    return ret;
                }
            }
            finally
            {
                gcHandle.Free();
            }
        }

        /// <summary>
        /// Cast a shape along direction and collect hits using the specified collector type.
        /// </summary>
        public bool CastShape(Shape shape,
            Mat4 transform,
            Float3 direction,
            in ShapeCastSettings settings,
            Float3 baseOffset,
            ICollection<ShapeCastResult> results,
            CollisionCollectorType collectorType = CollisionCollectorType.AllHit,
            BroadPhaseLayerFilter? broadPhaseLayerFilter = null,
            ObjectLayerFilter? objectLayerFilter = null,
            BodyFilter? bodyFilter = null,
            ShapeFilter? shapeFilter = null)
        {
            var gcHandle = GCHandle.Alloc(results);
            try
            {
                unsafe
                {
                    var m = *(JPH_Mat4*)&transform;
                    var d = *(JPH_Vec3*)&direction;
                    JPH_ShapeCastSettings native;
                    settings.ToNative(&native);
                    var bo = *(JPH_Vec3*)&baseOffset;
                    bool ret = Methods.JPH_NarrowPhaseQuery_CastShape2(
                        Ptr, (JPH_Shape*)shape.Handle, &m, &d,
                        &native,
                        &bo,
                        (JPH_CollisionCollectorType)collectorType,
#if NET5_0_OR_GREATER
                        &OnCastShapeResultCallback,
#else
                        (delegate* unmanaged[Cdecl]<void*, JPH_ShapeCastResult*, void>)s_pCastShape,
#endif
                        (void*)GCHandle.ToIntPtr(gcHandle),
                        (JPH_BroadPhaseLayerFilter*)(broadPhaseLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ObjectLayerFilter*)(objectLayerFilter?.Handle ?? IntPtr.Zero),
                        (JPH_BodyFilter*)(bodyFilter?.Handle ?? IntPtr.Zero),
                        (JPH_ShapeFilter*)(shapeFilter?.Handle ?? IntPtr.Zero)) != 0;
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

// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

namespace JoltPhysics
{
    public abstract class Shape: NativeHandle
    {
        internal Shape(IntPtr handle): base(handle, true)
        {
        }

        internal Shape(IntPtr handle, bool ownsHandle): base(handle, ownsHandle)
        {
        }

        public Float3 GetCenterOfMass()
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_Vec3 result;
                Methods.JPH_Shape_GetCenterOfMass((JPH_Shape*)Handle, &result);
                return new Float3(result.x, result.y, result.z);
            }
        }

        public ShapeType GetShapeType()
        {
            ThrowIfDisposed();
            unsafe { return (ShapeType)Methods.JPH_Shape_GetType((JPH_Shape*)Handle); }
        }

        public ShapeSubType GetShapeSubType()
        {
            ThrowIfDisposed();
            unsafe { return (ShapeSubType)Methods.JPH_Shape_GetSubType((JPH_Shape*)Handle); }
        }

        public BoundingBox GetLocalBounds()
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_AABox box;
                Methods.JPH_Shape_GetLocalBounds((JPH_Shape*)Handle, &box);
                return new BoundingBox(
                    new Float3(box.min.x, box.min.y, box.min.z),
                    new Float3(box.max.x, box.max.y, box.max.z));
            }
        }

        public float GetVolume()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_Shape_GetVolume((JPH_Shape*)Handle); }
        }

        public float GetInnerRadius()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_Shape_GetInnerRadius((JPH_Shape*)Handle); }
        }

        public MassProperties GetMassProperties()
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_MassProperties native;
                Methods.JPH_Shape_GetMassProperties((JPH_Shape*)Handle, &native);
                return MassProperties.FromNative(&native);
            }
        }

        public bool MustBeStatic()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_Shape_MustBeStatic((JPH_Shape*)Handle) != 0; }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_Shape_Destroy((JPH_Shape*)Handle);
            }
        }

        public uint GetSubShapeIDBitsRecursive()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_Shape_GetSubShapeIDBitsRecursive((JPH_Shape*)Handle); }
        }

        public BoundingBox GetWorldSpaceBounds(Mat4 centerOfMassTransform, Float3 scale)
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_AABox box;
                Methods.JPH_Shape_GetWorldSpaceBounds((JPH_Shape*)Handle,
                    (JPH_Mat4*)&centerOfMassTransform, (JPH_Vec3*)&scale, &box);
                return new BoundingBox(
                    new Float3(box.min.x, box.min.y, box.min.z),
                    new Float3(box.max.x, box.max.y, box.max.z));
            }
        }

        public Float3 GetSurfaceNormal(uint subShapeID, Float3 localPosition)
        {
            ThrowIfDisposed();
            unsafe
            {
                Float3 result;
                Methods.JPH_Shape_GetSurfaceNormal((JPH_Shape*)Handle, subShapeID,
                    (JPH_Vec3*)&localPosition, (JPH_Vec3*)&result);
                return result;
            }
        }

        public bool IsValidScale(Float3 scale)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_Shape_IsValidScale((JPH_Shape*)Handle, (JPH_Vec3*)&scale) != 0; }
        }

        public Float3 MakeScaleValid(Float3 scale)
        {
            ThrowIfDisposed();
            unsafe
            {
                Float3 result;
                Methods.JPH_Shape_MakeScaleValid((JPH_Shape*)Handle, (JPH_Vec3*)&scale, (JPH_Vec3*)&result);
                return result;
            }
        }

        public Shape ScaleShape(Float3 scale)
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = Methods.JPH_Shape_ScaleShape((JPH_Shape*)Handle, (JPH_Vec3*)&scale);
                return new GenericShape((IntPtr)ptr, true);
            }
        }

        public ulong UserData
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_Shape_GetUserData((JPH_Shape*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_Shape_SetUserData((JPH_Shape*)Handle, value); }
            }
        }

        public bool CastRay(Float3 origin, Float3 direction, out RayCastResult result)
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_RayCastResult native;
                var hit = Methods.JPH_Shape_CastRay((JPH_Shape*)Handle,
                    (JPH_Vec3*)&origin, (JPH_Vec3*)&direction, &native) != 0;
                result = new RayCastResult
                {
                    BodyID = new BodyID(native.bodyID),
                    Fraction = native.fraction,
                    SubShapeID2 = native.subShapeID2,
                };
                return hit;
            }
        }
    }
}
// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct PointConstraintSettings
    {
        public ConstraintSpace Space;
        public Float3 Point1;
        public Float3 Point2;

        public static PointConstraintSettings Default => new PointConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
        };

        internal JPH_PointConstraintSettings ToNative()
        {
            return new JPH_PointConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                point1 = Unsafe.As<Float3, JPH_Vec3>(ref Point1),
                point2 = Unsafe.As<Float3, JPH_Vec3>(ref Point2),
            };
        }
    }

    public sealed class PointConstraint : TwoBodyConstraint
    {
        internal PointConstraint(IntPtr handle) : base(handle) { }

        public static PointConstraint Create(in PointConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_PointConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new PointConstraint((IntPtr)ptr);
            }
        }

        public void SetPoint1(ConstraintSpace space, Float3 value)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_PointConstraint_SetPoint1((JPH_PointConstraint*)Handle, (JPH_ConstraintSpace)space, (JPH_Vec3*)&value); }
        }

        public void SetPoint2(ConstraintSpace space, Float3 value)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_PointConstraint_SetPoint2((JPH_PointConstraint*)Handle, (JPH_ConstraintSpace)space, (JPH_Vec3*)&value); }
        }

        public Float3 GetLocalSpacePoint1()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_PointConstraint_GetLocalSpacePoint1((JPH_PointConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetLocalSpacePoint2()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_PointConstraint_GetLocalSpacePoint2((JPH_PointConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetTotalLambdaPosition()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_PointConstraint_GetTotalLambdaPosition((JPH_PointConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }
    }
}

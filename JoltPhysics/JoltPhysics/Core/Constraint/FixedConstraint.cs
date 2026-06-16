// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct FixedConstraintSettings
    {
        public ConstraintSpace Space;
        public bool AutoDetectPoint;
        public Float3 Point1;
        public Float3 AxisX1;
        public Float3 AxisY1;
        public Float3 Point2;
        public Float3 AxisX2;
        public Float3 AxisY2;

        public static FixedConstraintSettings Default => new FixedConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
            AutoDetectPoint = true,
        };

        internal JPH_FixedConstraintSettings ToNative()
        {
            return new JPH_FixedConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                autoDetectPoint = (byte)(AutoDetectPoint ? 1 : 0),
                point1 = Unsafe.As<Float3, JPH_Vec3>(ref Point1),
                axisX1 = Unsafe.As<Float3, JPH_Vec3>(ref AxisX1),
                axisY1 = Unsafe.As<Float3, JPH_Vec3>(ref AxisY1),
                point2 = Unsafe.As<Float3, JPH_Vec3>(ref Point2),
                axisX2 = Unsafe.As<Float3, JPH_Vec3>(ref AxisX2),
                axisY2 = Unsafe.As<Float3, JPH_Vec3>(ref AxisY2),
            };
        }
    }

    public sealed class FixedConstraint : TwoBodyConstraint
    {
        internal FixedConstraint(IntPtr handle) : base(handle) { }

        public static FixedConstraint Create(in FixedConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_FixedConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new FixedConstraint((IntPtr)ptr);
            }
        }

        public Float3 GetTotalLambdaPosition()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_FixedConstraint_GetTotalLambdaPosition((JPH_FixedConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetTotalLambdaRotation()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_FixedConstraint_GetTotalLambdaRotation((JPH_FixedConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }
    }
}

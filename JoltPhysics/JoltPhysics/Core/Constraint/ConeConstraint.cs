// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct ConeConstraintSettings
    {
        public ConstraintSpace Space;
        public Float3 Point1;
        public Float3 TwistAxis1;
        public Float3 Point2;
        public Float3 TwistAxis2;
        public float HalfConeAngle;

        public static ConeConstraintSettings Default => new ConeConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
            TwistAxis1 = new Float3(0, 1, 0),
            TwistAxis2 = new Float3(0, 1, 0),
            HalfConeAngle = 0f,
        };

        internal JPH_ConeConstraintSettings ToNative()
        {
            return new JPH_ConeConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                point1 = Unsafe.As<Float3, JPH_Vec3>(ref Point1),
                twistAxis1 = Unsafe.As<Float3, JPH_Vec3>(ref TwistAxis1),
                point2 = Unsafe.As<Float3, JPH_Vec3>(ref Point2),
                twistAxis2 = Unsafe.As<Float3, JPH_Vec3>(ref TwistAxis2),
                halfConeAngle = HalfConeAngle,
            };
        }
    }

    public sealed class ConeConstraint : TwoBodyConstraint
    {
        internal ConeConstraint(IntPtr handle) : base(handle) { }

        public static ConeConstraint Create(in ConeConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_ConeConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new ConeConstraint((IntPtr)ptr);
            }
        }

        public void SetHalfConeAngle(float halfConeAngle)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_ConeConstraint_SetHalfConeAngle((JPH_ConeConstraint*)Handle, halfConeAngle); }
        }

        public float CosHalfConeAngle
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_ConeConstraint_GetCosHalfConeAngle((JPH_ConeConstraint*)Handle); }
            }
        }

        public Float3 GetTotalLambdaPosition()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_ConeConstraint_GetTotalLambdaPosition((JPH_ConeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public float GetTotalLambdaRotation()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_ConeConstraint_GetTotalLambdaRotation((JPH_ConeConstraint*)Handle); }
        }
    }
}

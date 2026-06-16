// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct SwingTwistConstraintSettings
    {
        public ConstraintSpace Space;
        public Float3 Position1;
        public Float3 TwistAxis1;
        public Float3 PlaneAxis1;
        public Float3 Position2;
        public Float3 TwistAxis2;
        public Float3 PlaneAxis2;
        public SwingType SwingType;
        public float NormalHalfConeAngle;
        public float PlaneHalfConeAngle;
        public float TwistMinAngle;
        public float TwistMaxAngle;
        public float MaxFrictionTorque;
        public MotorSettings SwingMotorSettings;
        public MotorSettings TwistMotorSettings;

        public static SwingTwistConstraintSettings Default => new SwingTwistConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
            TwistAxis1 = new Float3(1, 0, 0),
            PlaneAxis1 = new Float3(0, 1, 0),
            TwistAxis2 = new Float3(1, 0, 0),
            PlaneAxis2 = new Float3(0, 1, 0),
            NormalHalfConeAngle = MathF.PI,
            PlaneHalfConeAngle = MathF.PI,
            TwistMinAngle = -MathF.PI,
            TwistMaxAngle = MathF.PI,
        };

        internal JPH_SwingTwistConstraintSettings ToNative()
        {
            return new JPH_SwingTwistConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                position1 = Unsafe.As<Float3, JPH_Vec3>(ref Position1),
                twistAxis1 = Unsafe.As<Float3, JPH_Vec3>(ref TwistAxis1),
                planeAxis1 = Unsafe.As<Float3, JPH_Vec3>(ref PlaneAxis1),
                position2 = Unsafe.As<Float3, JPH_Vec3>(ref Position2),
                twistAxis2 = Unsafe.As<Float3, JPH_Vec3>(ref TwistAxis2),
                planeAxis2 = Unsafe.As<Float3, JPH_Vec3>(ref PlaneAxis2),
                swingType = (JPH_SwingType)SwingType,
                normalHalfConeAngle = NormalHalfConeAngle,
                planeHalfConeAngle = PlaneHalfConeAngle,
                twistMinAngle = TwistMinAngle,
                twistMaxAngle = TwistMaxAngle,
                maxFrictionTorque = MaxFrictionTorque,
                swingMotorSettings = SwingMotorSettings.ToNative(),
                twistMotorSettings = TwistMotorSettings.ToNative(),
            };
        }
    }

    public sealed class SwingTwistConstraint : TwoBodyConstraint
    {
        internal SwingTwistConstraint(IntPtr handle) : base(handle) { }

        public static SwingTwistConstraint Create(in SwingTwistConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_SwingTwistConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new SwingTwistConstraint((IntPtr)ptr);
            }
        }

        public float NormalHalfConeAngle
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SwingTwistConstraint_GetNormalHalfConeAngle((JPH_SwingTwistConstraint*)Handle); }
            }
        }

        public Float3 GetTotalLambdaPosition()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_SwingTwistConstraint_GetTotalLambdaPosition((JPH_SwingTwistConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public float GetTotalLambdaTwist()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SwingTwistConstraint_GetTotalLambdaTwist((JPH_SwingTwistConstraint*)Handle); }
        }

        public float GetTotalLambdaSwingY()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SwingTwistConstraint_GetTotalLambdaSwingY((JPH_SwingTwistConstraint*)Handle); }
        }

        public float GetTotalLambdaSwingZ()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SwingTwistConstraint_GetTotalLambdaSwingZ((JPH_SwingTwistConstraint*)Handle); }
        }

        public Float3 GetTotalLambdaMotor()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_SwingTwistConstraint_GetTotalLambdaMotor((JPH_SwingTwistConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }
    }
}

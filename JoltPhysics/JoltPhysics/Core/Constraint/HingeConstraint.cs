// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct HingeConstraintSettings
    {
        public ConstraintSpace Space;
        public Float3 Point1;
        public Float3 HingeAxis1;
        public Float3 NormalAxis1;
        public Float3 Point2;
        public Float3 HingeAxis2;
        public Float3 NormalAxis2;
        public float LimitsMin;
        public float LimitsMax;
        public SpringSettings LimitsSpringSettings;
        public float MaxFrictionTorque;
        public MotorSettings MotorSettings;

        public static HingeConstraintSettings Default => new HingeConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
            HingeAxis1 = new Float3(0, 1, 0),
            NormalAxis1 = new Float3(1, 0, 0),
            HingeAxis2 = new Float3(0, 1, 0),
            NormalAxis2 = new Float3(1, 0, 0),
            LimitsMin = -MathF.PI,
            LimitsMax = MathF.PI,
        };

        internal JPH_HingeConstraintSettings ToNative()
        {
            return new JPH_HingeConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                point1 = Unsafe.As<Float3, JPH_Vec3>(ref Point1),
                hingeAxis1 = Unsafe.As<Float3, JPH_Vec3>(ref HingeAxis1),
                normalAxis1 = Unsafe.As<Float3, JPH_Vec3>(ref NormalAxis1),
                point2 = Unsafe.As<Float3, JPH_Vec3>(ref Point2),
                hingeAxis2 = Unsafe.As<Float3, JPH_Vec3>(ref HingeAxis2),
                normalAxis2 = Unsafe.As<Float3, JPH_Vec3>(ref NormalAxis2),
                limitsMin = LimitsMin,
                limitsMax = LimitsMax,
                limitsSpringSettings = LimitsSpringSettings.ToNative(),
                maxFrictionTorque = MaxFrictionTorque,
                motorSettings = MotorSettings.ToNative(),
            };
        }
    }

    public sealed class HingeConstraint : TwoBodyConstraint
    {
        internal HingeConstraint(IntPtr handle) : base(handle) { }

        public static HingeConstraint Create(in HingeConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_HingeConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new HingeConstraint((IntPtr)ptr);
            }
        }

        public float CurrentAngle
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HingeConstraint_GetCurrentAngle((JPH_HingeConstraint*)Handle); }
            }
        }

        public float MaxFrictionTorque
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HingeConstraint_GetMaxFrictionTorque((JPH_HingeConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_HingeConstraint_SetMaxFrictionTorque((JPH_HingeConstraint*)Handle, value); }
            }
        }

        public MotorState MotorState
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return (MotorState)Methods.JPH_HingeConstraint_GetMotorState((JPH_HingeConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_HingeConstraint_SetMotorState((JPH_HingeConstraint*)Handle, (JPH_MotorState)value); }
            }
        }

        public float TargetAngularVelocity
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HingeConstraint_GetTargetAngularVelocity((JPH_HingeConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_HingeConstraint_SetTargetAngularVelocity((JPH_HingeConstraint*)Handle, value); }
            }
        }

        public float TargetAngle
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HingeConstraint_GetTargetAngle((JPH_HingeConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_HingeConstraint_SetTargetAngle((JPH_HingeConstraint*)Handle, value); }
            }
        }

        public void SetLimits(float min, float max)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_HingeConstraint_SetLimits((JPH_HingeConstraint*)Handle, min, max); }
        }

        public float LimitsMin
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HingeConstraint_GetLimitsMin((JPH_HingeConstraint*)Handle); }
            }
        }

        public float LimitsMax
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HingeConstraint_GetLimitsMax((JPH_HingeConstraint*)Handle); }
            }
        }

        public bool HasLimits
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_HingeConstraint_HasLimits((JPH_HingeConstraint*)Handle) != 0; }
            }
        }

        public MotorSettings GetMotorSettings()
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_MotorSettings native;
                Methods.JPH_HingeConstraint_GetMotorSettings((JPH_HingeConstraint*)Handle, &native);
                return JoltPhysics.MotorSettings.FromNative(native);
            }
        }

        public void SetMotorSettings(MotorSettings settings)
        {
            ThrowIfDisposed();
            unsafe
            {
                var native = settings.ToNative();
                Methods.JPH_HingeConstraint_SetMotorSettings((JPH_HingeConstraint*)Handle, &native);
            }
        }

        public Float3 GetLocalSpacePoint1()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_HingeConstraint_GetLocalSpacePoint1((JPH_HingeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetLocalSpacePoint2()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_HingeConstraint_GetLocalSpacePoint2((JPH_HingeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetLocalSpaceHingeAxis1()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_HingeConstraint_GetLocalSpaceHingeAxis1((JPH_HingeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetLocalSpaceHingeAxis2()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_HingeConstraint_GetLocalSpaceHingeAxis2((JPH_HingeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetLocalSpaceNormalAxis1()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_HingeConstraint_GetLocalSpaceNormalAxis1((JPH_HingeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetLocalSpaceNormalAxis2()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_HingeConstraint_GetLocalSpaceNormalAxis2((JPH_HingeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public SpringSettings LimitsSpringSettings
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_SpringSettings native;
                    Methods.JPH_HingeConstraint_GetLimitsSpringSettings((JPH_HingeConstraint*)Handle, &native);
                    return SpringSettings.FromNative(native);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    var native = value.ToNative();
                    Methods.JPH_HingeConstraint_SetLimitsSpringSettings((JPH_HingeConstraint*)Handle, &native);
                }
            }
        }

        public Float3 GetTotalLambdaPosition()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_HingeConstraint_GetTotalLambdaPosition((JPH_HingeConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public float GetTotalLambdaRotationLimits()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_HingeConstraint_GetTotalLambdaRotationLimits((JPH_HingeConstraint*)Handle); }
        }

        public float GetTotalLambdaMotor()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_HingeConstraint_GetTotalLambdaMotor((JPH_HingeConstraint*)Handle); }
        }
    }
}

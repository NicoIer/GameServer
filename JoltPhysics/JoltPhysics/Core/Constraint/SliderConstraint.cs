// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct SliderConstraintSettings
    {
        public ConstraintSpace Space;
        public bool AutoDetectPoint;
        public Float3 Point1;
        public Float3 SliderAxis1;
        public Float3 NormalAxis1;
        public Float3 Point2;
        public Float3 SliderAxis2;
        public Float3 NormalAxis2;
        public float LimitsMin;
        public float LimitsMax;
        public SpringSettings LimitsSpringSettings;
        public float MaxFrictionForce;
        public MotorSettings MotorSettings;

        public static SliderConstraintSettings Default => new SliderConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
            AutoDetectPoint = true,
            SliderAxis1 = new Float3(1, 0, 0),
            NormalAxis1 = new Float3(0, 1, 0),
            SliderAxis2 = new Float3(1, 0, 0),
            NormalAxis2 = new Float3(0, 1, 0),
            LimitsMin = float.MinValue,
            LimitsMax = float.MaxValue,
        };

        internal JPH_SliderConstraintSettings ToNative()
        {
            return new JPH_SliderConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                autoDetectPoint = (byte)(AutoDetectPoint ? 1 : 0),
                point1 = Unsafe.As<Float3, JPH_Vec3>(ref Point1),
                sliderAxis1 = Unsafe.As<Float3, JPH_Vec3>(ref SliderAxis1),
                normalAxis1 = Unsafe.As<Float3, JPH_Vec3>(ref NormalAxis1),
                point2 = Unsafe.As<Float3, JPH_Vec3>(ref Point2),
                sliderAxis2 = Unsafe.As<Float3, JPH_Vec3>(ref SliderAxis2),
                normalAxis2 = Unsafe.As<Float3, JPH_Vec3>(ref NormalAxis2),
                limitsMin = LimitsMin,
                limitsMax = LimitsMax,
                limitsSpringSettings = LimitsSpringSettings.ToNative(),
                maxFrictionForce = MaxFrictionForce,
                motorSettings = MotorSettings.ToNative(),
            };
        }
    }

    public sealed class SliderConstraint : TwoBodyConstraint
    {
        internal SliderConstraint(IntPtr handle) : base(handle) { }

        public static SliderConstraint Create(in SliderConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_SliderConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new SliderConstraint((IntPtr)ptr);
            }
        }

        public float CurrentPosition
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SliderConstraint_GetCurrentPosition((JPH_SliderConstraint*)Handle); }
            }
        }

        public float MaxFrictionForce
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SliderConstraint_GetMaxFrictionForce((JPH_SliderConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_SliderConstraint_SetMaxFrictionForce((JPH_SliderConstraint*)Handle, value); }
            }
        }

        public MotorState MotorState
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return (MotorState)Methods.JPH_SliderConstraint_GetMotorState((JPH_SliderConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_SliderConstraint_SetMotorState((JPH_SliderConstraint*)Handle, (JPH_MotorState)value); }
            }
        }

        public float TargetVelocity
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SliderConstraint_GetTargetVelocity((JPH_SliderConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_SliderConstraint_SetTargetVelocity((JPH_SliderConstraint*)Handle, value); }
            }
        }

        public float TargetPosition
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SliderConstraint_GetTargetPosition((JPH_SliderConstraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_SliderConstraint_SetTargetPosition((JPH_SliderConstraint*)Handle, value); }
            }
        }

        public void SetLimits(float min, float max)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_SliderConstraint_SetLimits((JPH_SliderConstraint*)Handle, min, max); }
        }

        public float LimitsMin
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SliderConstraint_GetLimitsMin((JPH_SliderConstraint*)Handle); }
            }
        }

        public float LimitsMax
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SliderConstraint_GetLimitsMax((JPH_SliderConstraint*)Handle); }
            }
        }

        public bool HasLimits
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_SliderConstraint_HasLimits((JPH_SliderConstraint*)Handle) != 0; }
            }
        }

        public MotorSettings GetMotorSettings()
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_MotorSettings native;
                Methods.JPH_SliderConstraint_GetMotorSettings((JPH_SliderConstraint*)Handle, &native);
                return JoltPhysics.MotorSettings.FromNative(native);
            }
        }

        public void SetMotorSettings(MotorSettings settings)
        {
            ThrowIfDisposed();
            unsafe
            {
                var native = settings.ToNative();
                Methods.JPH_SliderConstraint_SetMotorSettings((JPH_SliderConstraint*)Handle, &native);
            }
        }

        public SpringSettings LimitsSpringSettings
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_SpringSettings native;
                    Methods.JPH_SliderConstraint_GetLimitsSpringSettings((JPH_SliderConstraint*)Handle, &native);
                    return SpringSettings.FromNative(native);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    var native = value.ToNative();
                    Methods.JPH_SliderConstraint_SetLimitsSpringSettings((JPH_SliderConstraint*)Handle, &native);
                }
            }
        }

        public float GetTotalLambdaPositionLimits()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SliderConstraint_GetTotalLambdaPositionLimits((JPH_SliderConstraint*)Handle); }
        }

        public Float3 GetTotalLambdaRotation()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_SliderConstraint_GetTotalLambdaRotation((JPH_SliderConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public float GetTotalLambdaMotor()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SliderConstraint_GetTotalLambdaMotor((JPH_SliderConstraint*)Handle); }
        }
    }
}

// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct SixDOFConstraintSettings
    {
        public ConstraintSpace Space;
        public Float3 Position1;
        public Float3 AxisX1;
        public Float3 AxisY1;
        public Float3 Position2;
        public Float3 AxisX2;
        public Float3 AxisY2;
        public SwingType SwingType;

        public static SixDOFConstraintSettings Default => new SixDOFConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
            AxisX1 = new Float3(1, 0, 0),
            AxisY1 = new Float3(0, 1, 0),
            AxisX2 = new Float3(1, 0, 0),
            AxisY2 = new Float3(0, 1, 0),
        };

        internal JPH_SixDOFConstraintSettings ToNative()
        {
            return new JPH_SixDOFConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                position1 = Unsafe.As<Float3, JPH_Vec3>(ref Position1),
                axisX1 = Unsafe.As<Float3, JPH_Vec3>(ref AxisX1),
                axisY1 = Unsafe.As<Float3, JPH_Vec3>(ref AxisY1),
                position2 = Unsafe.As<Float3, JPH_Vec3>(ref Position2),
                axisX2 = Unsafe.As<Float3, JPH_Vec3>(ref AxisX2),
                axisY2 = Unsafe.As<Float3, JPH_Vec3>(ref AxisY2),
                swingType = (JPH_SwingType)SwingType,
            };
        }
    }

    public sealed class SixDOFConstraint : TwoBodyConstraint
    {
        internal SixDOFConstraint(IntPtr handle) : base(handle) { }

        public static SixDOFConstraint Create(in SixDOFConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_SixDOFConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new SixDOFConstraint((IntPtr)ptr);
            }
        }

        public float GetLimitsMin(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SixDOFConstraint_GetLimitsMin((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis); }
        }

        public float GetLimitsMax(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SixDOFConstraint_GetLimitsMax((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis); }
        }

        public bool IsFixedAxis(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SixDOFConstraint_IsFixedAxis((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis) != 0; }
        }

        public bool IsFreeAxis(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SixDOFConstraint_IsFreeAxis((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis) != 0; }
        }

        public Float3 TranslationLimitsMin
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_SixDOFConstraint_GetTranslationLimitsMin((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }

        public Float3 TranslationLimitsMax
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_SixDOFConstraint_GetTranslationLimitsMax((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }

        public Float3 RotationLimitsMin
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_SixDOFConstraint_GetRotationLimitsMin((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }

        public Float3 RotationLimitsMax
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_SixDOFConstraint_GetRotationLimitsMax((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
        }

        public Float3 GetTotalLambdaPosition()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_SixDOFConstraint_GetTotalLambdaPosition((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetTotalLambdaRotation()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_SixDOFConstraint_GetTotalLambdaRotation((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetTotalLambdaMotorTranslation()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_SixDOFConstraint_GetTotalLambdaMotorTranslation((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public Float3 GetTotalLambdaMotorRotation()
        {
            ThrowIfDisposed();
            unsafe { Float3 r; Methods.JPH_SixDOFConstraint_GetTotalLambdaMotorRotation((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&r); return r; }
        }

        public SpringSettings GetLimitsSpringSettings(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_SpringSettings native;
                Methods.JPH_SixDOFConstraint_GetLimitsSpringSettings((JPH_SixDOFConstraint*)Handle, &native, (JPH_SixDOFConstraintAxis)axis);
                return SpringSettings.FromNative(native);
            }
        }

        public void SetLimitsSpringSettings(SixDOFConstraintAxis axis, SpringSettings settings)
        {
            ThrowIfDisposed();
            unsafe
            {
                var native = settings.ToNative();
                Methods.JPH_SixDOFConstraint_SetLimitsSpringSettings((JPH_SixDOFConstraint*)Handle, &native, (JPH_SixDOFConstraintAxis)axis);
            }
        }

        public float GetMaxFriction(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_SixDOFConstraint_GetMaxFriction((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis); }
        }

        public void SetMaxFriction(SixDOFConstraintAxis axis, float friction)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_SixDOFConstraint_SetMaxFriction((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis, friction); }
        }

        public Quat GetRotationInConstraintSpace()
        {
            ThrowIfDisposed();
            unsafe { Quat r; Methods.JPH_SixDOFConstraint_GetRotationInConstraintSpace((JPH_SixDOFConstraint*)Handle, (JPH_Quat*)&r); return r; }
        }

        public MotorSettings GetMotorSettings(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_MotorSettings native;
                Methods.JPH_SixDOFConstraint_GetMotorSettings((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis, &native);
                return JoltPhysics.MotorSettings.FromNative(native);
            }
        }

        public void SetMotorState(SixDOFConstraintAxis axis, MotorState state)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_SixDOFConstraint_SetMotorState((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis, (JPH_MotorState)state); }
        }

        public MotorState GetMotorState(SixDOFConstraintAxis axis)
        {
            ThrowIfDisposed();
            unsafe { return (MotorState)Methods.JPH_SixDOFConstraint_GetMotorState((JPH_SixDOFConstraint*)Handle, (JPH_SixDOFConstraintAxis)axis); }
        }

        public Float3 TargetVelocityCS
        {
            get { ThrowIfDisposed(); unsafe { Float3 r; Methods.JPH_SixDOFConstraint_GetTargetVelocityCS((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&r); return r; } }
            set { ThrowIfDisposed(); unsafe { Methods.JPH_SixDOFConstraint_SetTargetVelocityCS((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&value); } }
        }

        public Float3 TargetAngularVelocityCS
        {
            get { ThrowIfDisposed(); unsafe { Float3 r; Methods.JPH_SixDOFConstraint_GetTargetAngularVelocityCS((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&r); return r; } }
            set { ThrowIfDisposed(); unsafe { Methods.JPH_SixDOFConstraint_SetTargetAngularVelocityCS((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&value); } }
        }

        public Float3 TargetPositionCS
        {
            get { ThrowIfDisposed(); unsafe { Float3 r; Methods.JPH_SixDOFConstraint_GetTargetPositionCS((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&r); return r; } }
            set { ThrowIfDisposed(); unsafe { Methods.JPH_SixDOFConstraint_SetTargetPositionCS((JPH_SixDOFConstraint*)Handle, (JPH_Vec3*)&value); } }
        }

        public Quat TargetOrientationCS
        {
            get { ThrowIfDisposed(); unsafe { Quat r; Methods.JPH_SixDOFConstraint_GetTargetOrientationCS((JPH_SixDOFConstraint*)Handle, (JPH_Quat*)&r); return r; } }
            set { ThrowIfDisposed(); unsafe { Methods.JPH_SixDOFConstraint_SetTargetOrientationCS((JPH_SixDOFConstraint*)Handle, (JPH_Quat*)&value); } }
        }

        public void SetTargetOrientationBS(Quat orientation)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_SixDOFConstraint_SetTargetOrientationBS((JPH_SixDOFConstraint*)Handle, (JPH_Quat*)&orientation); }
        }
    }
}

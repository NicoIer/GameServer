// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    public struct DistanceConstraintSettings
    {
        public ConstraintSpace Space;
        public Float3 Point1;
        public Float3 Point2;
        public float MinDistance;
        public float MaxDistance;
        public SpringSettings LimitsSpringSettings;

        public static DistanceConstraintSettings Default => new DistanceConstraintSettings
        {
            Space = ConstraintSpace.WorldSpace,
            MinDistance = -1f,
            MaxDistance = -1f,
        };

        internal JPH_DistanceConstraintSettings ToNative()
        {
            return new JPH_DistanceConstraintSettings
            {
                @base = new JPH_ConstraintSettings { enabled = 1 },
                space = (JPH_ConstraintSpace)Space,
                point1 = Unsafe.As<Float3, JPH_Vec3>(ref Point1),
                point2 = Unsafe.As<Float3, JPH_Vec3>(ref Point2),
                minDistance = MinDistance,
                maxDistance = MaxDistance,
                limitsSpringSettings = LimitsSpringSettings.ToNative(),
            };
        }
    }

    public sealed class DistanceConstraint : TwoBodyConstraint
    {
        internal DistanceConstraint(IntPtr handle) : base(handle) { }

        public static DistanceConstraint Create(in DistanceConstraintSettings settings,
            BodyLockInterface lockInterface, BodyID bodyID1, BodyID bodyID2)
        {
            unsafe
            {
                using var lock1 = lockInterface.LockWrite(bodyID1);
                using var lock2 = lockInterface.LockWrite(bodyID2);
                if (!lock1.Succeeded || !lock2.Succeeded)
                    throw new InvalidOperationException("Failed to lock bodies for constraint creation");

                var native = settings.ToNative();
                var ptr = Methods.JPH_DistanceConstraint_Create(&native, lock1.BodyPtr, lock2.BodyPtr);
                return new DistanceConstraint((IntPtr)ptr);
            }
        }

        public float MinDistance
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_DistanceConstraint_GetMinDistance((JPH_DistanceConstraint*)Handle); }
            }
        }

        public float MaxDistance
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_DistanceConstraint_GetMaxDistance((JPH_DistanceConstraint*)Handle); }
            }
        }

        public void SetDistance(float minDistance, float maxDistance)
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_DistanceConstraint_SetDistance((JPH_DistanceConstraint*)Handle, minDistance, maxDistance); }
        }

        public SpringSettings LimitsSpringSettings
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_SpringSettings native;
                    Methods.JPH_DistanceConstraint_GetLimitsSpringSettings((JPH_DistanceConstraint*)Handle, &native);
                    return SpringSettings.FromNative(native);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    var native = value.ToNative();
                    Methods.JPH_DistanceConstraint_SetLimitsSpringSettings((JPH_DistanceConstraint*)Handle, &native);
                }
            }
        }

        public float GetTotalLambdaPosition()
        {
            ThrowIfDisposed();
            unsafe { return Methods.JPH_DistanceConstraint_GetTotalLambdaPosition((JPH_DistanceConstraint*)Handle); }
        }
    }
}

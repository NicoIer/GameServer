// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;
using System.Runtime.CompilerServices;

namespace JoltPhysics
{
    /// <summary>
    /// 非拥有的 MotionProperties 包装。通过 Body.GetMotionProperties() 获取。
    /// 仅在持有 BodyLock 期间有效。
    /// </summary>
    public readonly unsafe struct MotionProperties
    {
        internal readonly JPH_MotionProperties* Ptr;

        internal MotionProperties(JPH_MotionProperties* ptr)
        {
            Ptr = ptr;
        }

        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Ptr != null;
        }

        public AllowedDOFs AllowedDOFs
        {
            get => (AllowedDOFs)Methods.JPH_MotionProperties_GetAllowedDOFs(Ptr);
        }

        public float LinearDamping
        {
            get => Methods.JPH_MotionProperties_GetLinearDamping(Ptr);
            set => Methods.JPH_MotionProperties_SetLinearDamping(Ptr, value);
        }

        public float AngularDamping
        {
            get => Methods.JPH_MotionProperties_GetAngularDamping(Ptr);
            set => Methods.JPH_MotionProperties_SetAngularDamping(Ptr, value);
        }

        public float InverseMass
        {
            get => Methods.JPH_MotionProperties_GetInverseMassUnchecked(Ptr);
            set => Methods.JPH_MotionProperties_SetInverseMass(Ptr, value);
        }

        public Float3 InverseInertiaDiagonal
        {
            get
            {
                Float3 result;
                Methods.JPH_MotionProperties_GetInverseInertiaDiagonal(Ptr, (JPH_Vec3*)&result);
                return result;
            }
        }

        public Quat InertiaRotation
        {
            get
            {
                Quat result;
                Methods.JPH_MotionProperties_GetInertiaRotation(Ptr, (JPH_Quat*)&result);
                return result;
            }
        }

        public void SetInverseInertia(Float3 diagonal, Quat rotation)
        {
            Methods.JPH_MotionProperties_SetInverseInertia(Ptr, (JPH_Vec3*)&diagonal, (JPH_Quat*)&rotation);
        }

        public void ScaleToMass(float mass)
        {
            Methods.JPH_MotionProperties_ScaleToMass(Ptr, mass);
        }

        public void SetMassProperties(AllowedDOFs allowedDOFs, MassProperties massProperties)
        {
            var native = massProperties.ToNative();
            Methods.JPH_MotionProperties_SetMassProperties(Ptr, (JPH_AllowedDOFs)allowedDOFs, &native);
        }
    }
}

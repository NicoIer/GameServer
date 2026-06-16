// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class BodyCreationSettings : NativeHandle
    {
        public BodyCreationSettings(Shape shape, Float3 position, Quat rotation,
            MotionType motionType, ObjectLayer objectLayer)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                var pos = new JPH_Vec3 { x = position.x, y = position.y, z = position.z };
                var rot = new JPH_Quat { x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w };
                Handle = (IntPtr)Methods.JPH_BodyCreationSettings_Create3(
                    (JPH_Shape*)shape.Handle, &pos, &rot,
                    (JPH_MotionType)motionType, objectLayer.Value);
            }
        }

        public float Friction
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetFriction(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetFriction(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float Restitution
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetRestitution(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetRestitution(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float LinearDamping
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetLinearDamping(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetLinearDamping(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float AngularDamping
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetAngularDamping(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetAngularDamping(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public bool IsSensor
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetIsSensor(
                        (JPH_BodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetIsSensor(
                        (JPH_BodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public AllowedDOFs AllowedDOFs
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return (AllowedDOFs)Methods.JPH_BodyCreationSettings_GetAllowedDOFs(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetAllowedDOFs(
                        (JPH_BodyCreationSettings*)Handle, (JPH_AllowedDOFs)value);
                }
            }
        }

        public bool AllowSleeping
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetAllowSleeping(
                        (JPH_BodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetAllowSleeping(
                        (JPH_BodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public MotionQuality MotionQuality
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return (MotionQuality)Methods.JPH_BodyCreationSettings_GetMotionQuality(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetMotionQuality(
                        (JPH_BodyCreationSettings*)Handle, (JPH_MotionQuality)value);
                }
            }
        }

        public float GravityFactor
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetGravityFactor(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetGravityFactor(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public Float3 Position
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_BodyCreationSettings_GetPosition(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetPosition(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Vec3*)&value);
                }
            }
        }

        public Quat Rotation
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Quat result;
                    Methods.JPH_BodyCreationSettings_GetRotation(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Quat*)&result);
                    return result;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetRotation(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Quat*)&value);
                }
            }
        }

        public Float3 LinearVelocity
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_BodyCreationSettings_GetLinearVelocity(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetLinearVelocity(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Vec3*)&value);
                }
            }
        }

        public Float3 AngularVelocity
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    Float3 result;
                    Methods.JPH_BodyCreationSettings_GetAngularVelocity(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetAngularVelocity(
                        (JPH_BodyCreationSettings*)Handle, (JPH_Vec3*)&value);
                }
            }
        }

        public ulong UserData
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetUserData(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetUserData(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public ObjectLayer ObjectLayer
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return new ObjectLayer(Methods.JPH_BodyCreationSettings_GetObjectLayer(
                        (JPH_BodyCreationSettings*)Handle));
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetObjectLayer(
                        (JPH_BodyCreationSettings*)Handle, value.Value);
                }
            }
        }

        public MotionType MotionType
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return (MotionType)Methods.JPH_BodyCreationSettings_GetMotionType(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetMotionType(
                        (JPH_BodyCreationSettings*)Handle, (JPH_MotionType)value);
                }
            }
        }

        public bool AllowDynamicOrKinematic
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetAllowDynamicOrKinematic(
                        (JPH_BodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetAllowDynamicOrKinematic(
                        (JPH_BodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public bool CollideKinematicVsNonDynamic
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetCollideKinematicVsNonDynamic(
                        (JPH_BodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetCollideKinematicVsNonDynamic(
                        (JPH_BodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public bool UseManifoldReduction
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetUseManifoldReduction(
                        (JPH_BodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetUseManifoldReduction(
                        (JPH_BodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public bool ApplyGyroscopicForce
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetApplyGyroscopicForce(
                        (JPH_BodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetApplyGyroscopicForce(
                        (JPH_BodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public bool EnhancedInternalEdgeRemoval
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetEnhancedInternalEdgeRemoval(
                        (JPH_BodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetEnhancedInternalEdgeRemoval(
                        (JPH_BodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public float MaxLinearVelocity
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetMaxLinearVelocity(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetMaxLinearVelocity(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float MaxAngularVelocity
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetMaxAngularVelocity(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetMaxAngularVelocity(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public uint NumVelocityStepsOverride
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetNumVelocityStepsOverride(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetNumVelocityStepsOverride(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public uint NumPositionStepsOverride
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetNumPositionStepsOverride(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetNumPositionStepsOverride(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public OverrideMassProperties OverrideMassProperties
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return (OverrideMassProperties)Methods.JPH_BodyCreationSettings_GetOverrideMassProperties(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetOverrideMassProperties(
                        (JPH_BodyCreationSettings*)Handle, (JPH_OverrideMassProperties)value);
                }
            }
        }

        public float InertiaMultiplier
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetInertiaMultiplier(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetInertiaMultiplier(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public MassProperties MassPropertiesOverride
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_MassProperties native;
                    Methods.JPH_BodyCreationSettings_GetMassPropertiesOverride(
                        (JPH_BodyCreationSettings*)Handle, &native);
                    return MassProperties.FromNative(&native);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    var native = value.ToNative();
                    Methods.JPH_BodyCreationSettings_SetMassPropertiesOverride(
                        (JPH_BodyCreationSettings*)Handle, &native);
                }
            }
        }

        public CollisionGroup CollisionGroup
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_CollisionGroup native;
                    Methods.JPH_BodyCreationSettings_GetCollisionGroup(
                        (JPH_BodyCreationSettings*)Handle, &native);
                    return CollisionGroup.FromNative(native);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    value.ToNative(out var native);
                    Methods.JPH_BodyCreationSettings_SetCollisionGroup(
                        (JPH_BodyCreationSettings*)Handle, &native);
                }
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_BodyCreationSettings_Destroy((JPH_BodyCreationSettings*)Handle);
            }
        }
    }
}

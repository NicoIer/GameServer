// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public sealed class SoftBodyCreationSettings : NativeHandle
    {
        public SoftBodyCreationSettings() : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_SoftBodyCreationSettings_Create();
            }
        }

        public SoftBodyCreationSettings(SoftBodySharedSettings sharedSettings, Float3 position, Quat rotation, ObjectLayer objectLayer)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                var pos = new JPH_Vec3 { x = position.x, y = position.y, z = position.z };
                var rot = new JPH_Quat { x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w };
                Handle = (IntPtr)Methods.JPH_SoftBodyCreationSettings_Create2(
                    (JPH_SoftBodySharedSettings*)sharedSettings.Handle,
                    &pos, &rot, objectLayer.Value);
            }
        }

        public void SetSettings(SoftBodySharedSettings sharedSettings)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_SoftBodyCreationSettings_SetSettings(
                    (JPH_SoftBodyCreationSettings*)Handle,
                    (JPH_SoftBodySharedSettings*)sharedSettings.Handle);
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
                    Methods.JPH_SoftBodyCreationSettings_GetPosition(
                        (JPH_SoftBodyCreationSettings*)Handle, (JPH_Vec3*)&result);
                    return result;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetPosition(
                        (JPH_SoftBodyCreationSettings*)Handle, (JPH_Vec3*)&value);
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
                    Methods.JPH_SoftBodyCreationSettings_GetRotation(
                        (JPH_SoftBodyCreationSettings*)Handle, (JPH_Quat*)&result);
                    return result;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetRotation(
                        (JPH_SoftBodyCreationSettings*)Handle, (JPH_Quat*)&value);
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
                    return Methods.JPH_SoftBodyCreationSettings_GetUserData(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetUserData(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
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
                    return new ObjectLayer(Methods.JPH_SoftBodyCreationSettings_GetObjectLayer(
                        (JPH_SoftBodyCreationSettings*)Handle));
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetObjectLayer(
                        (JPH_SoftBodyCreationSettings*)Handle, value.Value);
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
                    Methods.JPH_SoftBodyCreationSettings_GetCollisionGroup(
                        (JPH_SoftBodyCreationSettings*)Handle, &native);
                    return CollisionGroup.FromNative(native);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    value.ToNative(out var native);
                    Methods.JPH_SoftBodyCreationSettings_SetCollisionGroup(
                        (JPH_SoftBodyCreationSettings*)Handle, &native);
                }
            }
        }

        public uint NumIterations
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodyCreationSettings_GetNumIterations(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetNumIterations(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
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
                    return Methods.JPH_SoftBodyCreationSettings_GetLinearDamping(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetLinearDamping(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
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
                    return Methods.JPH_SoftBodyCreationSettings_GetMaxLinearVelocity(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetMaxLinearVelocity(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
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
                    return Methods.JPH_SoftBodyCreationSettings_GetRestitution(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetRestitution(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
                }
            }
        }

        public float Friction
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodyCreationSettings_GetFriction(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetFriction(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
                }
            }
        }

        public float Pressure
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodyCreationSettings_GetPressure(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetPressure(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
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
                    return Methods.JPH_SoftBodyCreationSettings_GetGravityFactor(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetGravityFactor(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
                }
            }
        }

        public float VertexRadius
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodyCreationSettings_GetVertexRadius(
                        (JPH_SoftBodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetVertexRadius(
                        (JPH_SoftBodyCreationSettings*)Handle, value);
                }
            }
        }

        public bool UpdatePosition
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodyCreationSettings_GetUpdatePosition(
                        (JPH_SoftBodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetUpdatePosition(
                        (JPH_SoftBodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public bool MakeRotationIdentity
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodyCreationSettings_GetMakeRotationIdentity(
                        (JPH_SoftBodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetMakeRotationIdentity(
                        (JPH_SoftBodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
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
                    return Methods.JPH_SoftBodyCreationSettings_GetAllowSleeping(
                        (JPH_SoftBodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetAllowSleeping(
                        (JPH_SoftBodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        public bool FacesDoubleSided
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SoftBodyCreationSettings_GetFacesDoubleSided(
                        (JPH_SoftBodyCreationSettings*)Handle) != 0;
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_SoftBodyCreationSettings_SetFacesDoubleSided(
                        (JPH_SoftBodyCreationSettings*)Handle, (byte)(value ? 1 : 0));
                }
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_SoftBodyCreationSettings_Destroy((JPH_SoftBodyCreationSettings*)Handle);
            }
        }
    }
}

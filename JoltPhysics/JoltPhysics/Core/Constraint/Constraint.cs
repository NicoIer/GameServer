// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public abstract class Constraint : NativeHandle
    {
        internal Constraint(IntPtr handle) : base(handle, true) { }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_Constraint_GetEnabled((JPH_Constraint*)Handle) != 0; }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_Constraint_SetEnabled((JPH_Constraint*)Handle, (byte)(value ? 1 : 0)); }
            }
        }

        public uint ConstraintPriority
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_Constraint_GetConstraintPriority((JPH_Constraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_Constraint_SetConstraintPriority((JPH_Constraint*)Handle, value); }
            }
        }

        public uint NumVelocityStepsOverride
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_Constraint_GetNumVelocityStepsOverride((JPH_Constraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_Constraint_SetNumVelocityStepsOverride((JPH_Constraint*)Handle, value); }
            }
        }

        public uint NumPositionStepsOverride
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_Constraint_GetNumPositionStepsOverride((JPH_Constraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_Constraint_SetNumPositionStepsOverride((JPH_Constraint*)Handle, value); }
            }
        }

        public ulong UserData
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_Constraint_GetUserData((JPH_Constraint*)Handle); }
            }
            set
            {
                ThrowIfDisposed();
                unsafe { Methods.JPH_Constraint_SetUserData((JPH_Constraint*)Handle, value); }
            }
        }

        public ConstraintType Type
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return (ConstraintType)Methods.JPH_Constraint_GetType((JPH_Constraint*)Handle); }
            }
        }

        public ConstraintSubType SubType
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return (ConstraintSubType)Methods.JPH_Constraint_GetSubType((JPH_Constraint*)Handle); }
            }
        }

        public bool IsActive
        {
            get
            {
                ThrowIfDisposed();
                unsafe { return Methods.JPH_Constraint_IsActive((JPH_Constraint*)Handle) != 0; }
            }
        }

        public void NotifyShapeChanged(BodyID bodyID, Float3 deltaCOM)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_Constraint_NotifyShapeChanged(
                    (JPH_Constraint*)Handle, bodyID.Value, (JPH_Vec3*)&deltaCOM);
            }
        }

        public void ResetWarmStart()
        {
            ThrowIfDisposed();
            unsafe { Methods.JPH_Constraint_ResetWarmStart((JPH_Constraint*)Handle); }
        }

        protected override void DestroyNative()
        {
            unsafe { Methods.JPH_Constraint_Destroy((JPH_Constraint*)Handle); }
        }
    }
}

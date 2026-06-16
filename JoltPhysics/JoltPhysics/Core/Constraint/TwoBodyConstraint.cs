// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System;

namespace JoltPhysics
{
    public abstract class TwoBodyConstraint : Constraint
    {
        internal TwoBodyConstraint(IntPtr handle) : base(handle) { }

        public unsafe Body GetBody1()
        {
            ThrowIfDisposed();
            return new Body(Methods.JPH_TwoBodyConstraint_GetBody1((JPH_TwoBodyConstraint*)Handle));
        }

        public unsafe Body GetBody2()
        {
            ThrowIfDisposed();
            return new Body(Methods.JPH_TwoBodyConstraint_GetBody2((JPH_TwoBodyConstraint*)Handle));
        }

        public Mat4 GetConstraintToBody1Matrix()
        {
            ThrowIfDisposed();
            unsafe
            {
                Mat4 result;
                Methods.JPH_TwoBodyConstraint_GetConstraintToBody1Matrix(
                    (JPH_TwoBodyConstraint*)Handle, (JPH_Mat4*)&result);
                return result;
            }
        }

        public Mat4 GetConstraintToBody2Matrix()
        {
            ThrowIfDisposed();
            unsafe
            {
                Mat4 result;
                Methods.JPH_TwoBodyConstraint_GetConstraintToBody2Matrix(
                    (JPH_TwoBodyConstraint*)Handle, (JPH_Mat4*)&result);
                return result;
            }
        }
    }
}
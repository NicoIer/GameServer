// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System.Runtime.InteropServices;

namespace JoltPhysics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MassProperties
    {
        public float Mass;
        public Mat4 Inertia;

        internal unsafe JPH_MassProperties ToNative()
        {
            JPH_MassProperties result;
            result.mass = Mass;
            fixed (Mat4* src = &Inertia)
            {
                result.inertia = *(JPH_Mat4*)src;
            }
            return result;
        }

        internal static unsafe MassProperties FromNative(JPH_MassProperties* native)
        {
            MassProperties result;
            result.Mass = native->mass;
            result.Inertia = *(Mat4*)&native->inertia;
            return result;
        }
    }
}

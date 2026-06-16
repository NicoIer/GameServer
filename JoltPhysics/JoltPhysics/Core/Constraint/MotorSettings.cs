// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System.Runtime.InteropServices;

namespace JoltPhysics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MotorSettings
    {
        public SpringSettings SpringSettings;
        public float MinForceLimit;
        public float MaxForceLimit;
        public float MinTorqueLimit;
        public float MaxTorqueLimit;

        internal JPH_MotorSettings ToNative() => new JPH_MotorSettings
        {
            springSettings = SpringSettings.ToNative(),
            minForceLimit = MinForceLimit,
            maxForceLimit = MaxForceLimit,
            minTorqueLimit = MinTorqueLimit,
            maxTorqueLimit = MaxTorqueLimit,
        };

        internal static MotorSettings FromNative(JPH_MotorSettings native) => new MotorSettings
        {
            SpringSettings = JoltPhysics.SpringSettings.FromNative(native.springSettings),
            MinForceLimit = native.minForceLimit,
            MaxForceLimit = native.maxForceLimit,
            MinTorqueLimit = native.minTorqueLimit,
            MaxTorqueLimit = native.maxTorqueLimit,
        };
    }
}

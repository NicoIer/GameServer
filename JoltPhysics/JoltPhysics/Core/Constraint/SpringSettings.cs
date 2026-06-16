// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System.Runtime.InteropServices;

namespace JoltPhysics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpringSettings
    {
        public SpringMode Mode;
        public float FrequencyOrStiffness;
        public float Damping;

        public static SpringSettings Default => new SpringSettings
        {
            Mode = SpringMode.FrequencyAndDamping,
            FrequencyOrStiffness = 0f,
            Damping = 0f,
        };

        internal JPH_SpringSettings ToNative() => new JPH_SpringSettings
        {
            mode = (JPH_SpringMode)Mode,
            frequencyOrStiffness = FrequencyOrStiffness,
            damping = Damping,
        };

        internal static SpringSettings FromNative(JPH_SpringSettings native) => new SpringSettings
        {
            Mode = (SpringMode)native.mode,
            FrequencyOrStiffness = native.frequencyOrStiffness,
            Damping = native.damping,
        };
    }
}
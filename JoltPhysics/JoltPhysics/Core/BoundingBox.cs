// Copyright (c)2026 NicoIer@163.com All Rights Reserved.

using System.Runtime.InteropServices;

namespace JoltPhysics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundingBox
    {
        public Float3 Min;
        public Float3 Max;

        public BoundingBox(Float3 min, Float3 max)
        {
            Min = min;
            Max = max;
        }

        public Float3 Center => new Float3(
            (Min.x + Max.x) * 0.5f,
            (Min.y + Max.y) * 0.5f,
            (Min.z + Max.z) * 0.5f);

        public Float3 Extent => new Float3(
            (Max.x - Min.x) * 0.5f,
            (Max.y - Min.y) * 0.5f,
            (Max.z - Min.z) * 0.5f);
    }
}

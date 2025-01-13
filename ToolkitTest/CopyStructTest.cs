using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ToolkitTest;

public class CopyStructTest
{
    public struct float3
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OptimizerInput
    {
        public readonly DistanceRange lod0;
        public readonly DistanceRange lod1;
        public readonly DistanceRange lod2;
        public readonly DistanceRange cull;
        public readonly float3 position;

        public OptimizerInput(DistanceRange lod0, DistanceRange lod1, DistanceRange lod2,
            DistanceRange cull, float3 position)
        {
            this.lod0 = lod0;
            this.lod1 = lod1;
            this.lod2 = lod2;
            this.cull = cull;
            this.position = position;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct DistanceRange
    {
        public float min;
        public float max;

        public static DistanceRange Near = new DistanceRange(0, 10);
        public static DistanceRange Middle = new DistanceRange(10, 20);
        public static DistanceRange Far = new DistanceRange(20, 30);
        public static DistanceRange Cull = new DistanceRange(30, float.MaxValue);

        public DistanceRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [Test]
    public void TestProfiler()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count = 100000;
        var array = new OptimizerInput[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = new OptimizerInput(DistanceRange.Near, DistanceRange.Middle, DistanceRange.Far,
                DistanceRange.Cull, new float3 { x = 1, y = 2, z = 3 });
        }

        stopwatch.Stop();
        TestContext.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}ms");
    }
}
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestProject;

public class InlineProfiler
{
    public static float Clamp01(float value)
    {
        if (value < 0) return 0;
        if (value > 1) return 1;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp01Inline(float value)
    {
        if (value < 0) return 0;
        if (value > 1) return 1;
        return value;
    }
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Test1()
    {
        Stopwatch stopwatch = new Stopwatch();
        int count = 10000000;
        stopwatch.Start();
        for (int i = 0; i < count; i++)
        {
            var value = Clamp01(i);
        }
        stopwatch.Stop();
        TestContext.WriteLine($"{stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Restart();
        for (int i = 0; i < count; i++)
        {
            var value = Clamp01(i);
        }
        stopwatch.Stop();
        TestContext.WriteLine($"{stopwatch.ElapsedMilliseconds}ms");

    }
}
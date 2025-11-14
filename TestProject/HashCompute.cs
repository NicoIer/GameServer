using System.Diagnostics;

namespace TestProject;

public class HashCompute
{
    [Test]
    public void TestBoolArrayHash()
    {
        Stopwatch stopwatch = new Stopwatch();
        Random random = new Random();
        const int arraySize = 1000000;
        bool[] boolArray = new bool[arraySize];
        const int iterations = 1000;
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            boolArray[random.Next(0, boolArray.Length)] = true;
            int hash = ComputeHash(boolArray);
        }

        stopwatch.Stop();
        TestContext.WriteLine(stopwatch.ElapsedMilliseconds / iterations);
    }

    private int ComputeHash(bool[] array)
    {
        if (array == null) return 0;
        int hash = array.Length;
        for (int i = 0; i < array.Length; i++)
        {
            // 使用位运算组合哈希值
            hash = unchecked(hash * 31 + (array[i] ? 1 : 0));
        }

        return hash;
    }
}
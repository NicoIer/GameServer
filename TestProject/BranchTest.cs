using System.Diagnostics;

public class BranchTest
{
    const int arraySize = 10000;

    const int iterations = 1000000;

    [Test]
    public void Test1()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Random random = new Random();
        double[] data = new double[arraySize];
        for (int i = 0; i < arraySize; i++)
        {
            data[i] = random.NextDouble();
        }

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < arraySize; j++)
            {
                double value = data[j];
                if (value < 0.8)
                {
                    DoSomethingSample(in value);
                }
                else
                {
                    DoSomethingComplex(in value);
                }
            }
        }

        stopwatch.Stop();
        TestContext.WriteLine(stopwatch.ElapsedMilliseconds);
    }

    [Test]
    public void Test2()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Random random = new Random();
        double[] data = new double[arraySize];
        for (int i = 0; i < arraySize; i++)
        {
            data[i] = random.NextDouble();
        }

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < arraySize; j++)
            {
                double value = data[j];
                if (value > 0.8)
                {
                    DoSomethingSample(in value);
                }
                else
                {
                    DoSomethingComplex(in value);
                }
            }
        }

        stopwatch.Stop();
        TestContext.WriteLine(stopwatch.ElapsedMilliseconds);
    }

    private double DoSomethingSample(in double value)
    {
        return value * value;
    }

    public double DoSomethingComplex(in double value)
    {
        return Math.Sqrt(value) + Math.Log(value + 1);
    }
}
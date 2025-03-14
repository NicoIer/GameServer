using System.Diagnostics;
using NUnit.Framework.Internal;

namespace TestProject;

public struct TemplateStruct
{
    public int id;
    public int lod;
}

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Stopwatch stopwatch = new Stopwatch();
        int count = 1000000;
        TemplateStruct[] test = new TemplateStruct[count];
        for (int i = 0; i < count; i++)
        {
            test[i] = (new TemplateStruct { id = i, lod = i });
        }

        stopwatch.Start();
        for (var i = 0; i < count; i++)
        {
            var data = test[i];
            data.id = 0;
            data.lod = 0;
            test[i] = data;
        }

        stopwatch.Stop();
        // 写日志
        Assert.Warn($"{stopwatch.ElapsedMilliseconds}ms");

        stopwatch.Restart();
        for (var i = 0; i < count; i++)
        {
            ref var data = ref test[i];
            data.id = 0;
            data.lod = 0;
            test[i] = data;
        }

        Assert.Warn($"{stopwatch.ElapsedMilliseconds}ms");
    }
}
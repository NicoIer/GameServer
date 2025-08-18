using UnityToolkit;

namespace ToolkitTest;

public class PriorityQueueTests
{
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        SimplePriorityQueue<float> queue = new SimplePriorityQueue<float>();
        queue.Enqueue(1, 1);
        queue.Enqueue(-1, -1);
        queue.Enqueue(4, 4);
        queue.Enqueue(5, -1);

        while (queue.Count!=0)
        {
            TestContext.WriteLine(queue.Dequeue().ToString());
        }
    }
}
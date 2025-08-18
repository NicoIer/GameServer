using GameCore.Physics;
using JoltServer;

namespace TestProject;

public class FrameStepTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        const int bufferSize = 16;
        FrameStep frameStep = new FrameStep(bufferSize);
        List<FrameInput> inputs = new List<FrameInput>();
        for (int i = 0; i < 1024; i++)
        {
            var input = new FrameInput();
            inputs.Add(input);
            frameStep.Accept(0, i, inputs[i]);
            frameStep.Accept(0, i + 1, inputs[i]);

            bool accept = frameStep.Accept(0, i + bufferSize + 1, inputs[i]);
            Assert.That(accept == false);
            var result = frameStep.Step();
            Assert.That(frameStep.future.Size == 0);
            Assert.That(frameStep.currentFrame == i + 1);
            if (i < bufferSize)
            {
                Assert.That(frameStep.histroy.Count == i + 1);
            }
            else
            {
                Assert.That(frameStep.histroy.Count == bufferSize);
            }
        }
    }
}
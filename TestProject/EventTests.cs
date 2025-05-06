namespace TestProject;

[TestFixture]
public class EventTests
{
    [Test]
    public void TestPriority()
    {
        Action call = () => { };
        for (int i = 0; i < 10; i++)
        {
            var i1 = i;
            call += () => { TestContext.Out.WriteLine(i1); };
        }

        call();
    }
}
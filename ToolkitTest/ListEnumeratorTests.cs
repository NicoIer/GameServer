using UnityToolkit.Collections;

namespace ToolkitTest;

public class ListEnumeratorTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void TestListEnumerator()
    {
        var l1 = new List<int> { 1, 2, 3, 4, 5 };
        var l2 = new List<int> { 1, 2, 3 };
        var l3 = new List<int>();
        var l4 = new List<int> { 1, 2, 3 };
        var l5 = new List<int>();
        var l6 = new List<int>();

        var e = new ListEnumerator<int>(l1, l2, l3, l4, l5, l6);
        Assert.That(e.length, Is.EqualTo(l1.Count + l2.Count + l3.Count + l4.Count + l5.Count + l6.Count));
        var current = e.Current;
        Assert.That(current, Is.EqualTo(default(int)));
        var r = new List<int>();
        while (e.MoveNext())
        {
            current = e.Current;
            r.Add(current);
        }

        Assert.IsTrue(r[0] == 1);
        Assert.IsTrue(r[1] == 2);
        Assert.IsTrue(r[2] == 3);
        Assert.IsTrue(r[3] == 4);
        Assert.IsTrue(r[4] == 5);
        Assert.IsTrue(r[5] == 1);
        Assert.IsTrue(r[6] == 2);
        Assert.IsTrue(r[7] == 3);
        Assert.IsTrue(r[8] == 1);
        Assert.IsTrue(r[9] == 2);
        Assert.IsTrue(r[10] == 3);
        Assert.IsFalse(e.MoveNext());
    }
}
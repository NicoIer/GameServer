using FishGame;

namespace FishGameTest;

public class GlobalTests
{
    [Test]
    public void TestGlobal()
    {
        Global.Singleton.Add(new GameDatabase());
        Assert.IsTrue(Global.Singleton.Get<GameDatabase>() != null);
    }

}
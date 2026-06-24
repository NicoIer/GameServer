namespace GameServer.Core.Systems;

public interface ISystem
{
    void OnCreate();
    void Update(in long deltaTimeMs, in int frame, in long timeNowMs);
    void OnDestroy();
}

public interface IWorld
{
}

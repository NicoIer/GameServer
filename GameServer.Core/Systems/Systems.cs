namespace GameServer.Core.Systems;

public interface ISystem
{
}

public interface IWorld
{
}

public interface ISystem<in TWorld> : ISystem where TWorld : IWorld
{
    void OnCreate(TWorld world);
    void OnUpdate(in long deltaTimeMs, in long timeNowMs, in int frame, TWorld world);
    void OnDestroy(TWorld world);
}
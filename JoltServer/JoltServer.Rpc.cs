using GameCore.Jolt;
using JoltPhysicsSharp;

namespace JoltServer;

public partial class JoltServer
{
    // TODO 添加一个兴趣机制 让客户端只关心自己想要关心的碰撞
    private void HandleRpc()
    {
        _app.physicsWorld.physicsSystem.OnContactAdded += OnContactAddedRpc;
        _app.physicsWorld.physicsSystem.OnContactRemoved += OnContactRemovedRpc;
        _app.physicsWorld.physicsSystem.OnContactPersisted += OnContactPersistedRpc;
    }

    private void OnContactPersistedRpc(PhysicsSystem system, in Body body1, in Body body2, in ContactManifold manifold,
        in ContactSettings settings)
    {
        var rpc = new RpcContactPersisted(body1.ID.ID,body2.ID.ID);
        _server.SendToAll(rpc);
    }

    private void OnContactRemovedRpc(PhysicsSystem system, ref SubShapeIDPair subShapePair)
    {
        var rpc = new RpcContactRemoved(subShapePair.Body1ID.ID,subShapePair.Body2ID);
        _server.SendToAll(rpc);
    }

    private void OnContactAddedRpc(PhysicsSystem system, in Body body1, in Body body2, in ContactManifold manifold,
        in ContactSettings settings)
    {
        
        var rpc = new RpcContactAdded(body1.ID.ID,body2.ID.ID);
        _server.SendToAll(rpc);
    }
}
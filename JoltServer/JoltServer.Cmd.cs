using System.Diagnostics;
using GameCore.Physics;
using JoltPhysicsSharp;
using MemoryPack;
using Network;
using Serilog;
using UnityToolkit;

namespace JoltServer;

public partial class JoltServer
{
    private void HandleCmd()
    {
        // Cmd
        _server.messageHandler.Add<CmdSpawnBody>(OnCmdSpawnBody);
        // _server.messageHandler.Add<CmdSpawnBox>(OnCmdSpawnBox);
        // _server.messageHandler.Add<CmdSpawnPlane>(OnCmdSpawnPlane);


        _server.messageHandler.Add<CmdDestroy>(OnCmdDestroy);
        _server.messageHandler.Add<CmdBodyState>(OnCmdBodyState);
    }

    private void OnCmdSpawnBody(in int connectionId, in CmdSpawnBody message)
    {
        Debug.Assert(float.IsNaN(message.rotation.X) == false);
        Debug.Assert(float.IsNaN(message.rotation.Y) == false);
        Debug.Assert(float.IsNaN(message.rotation.Z) == false);
        Debug.Assert(float.IsNaN(message.rotation.W) == false);
        Log.Information($"客户端{connectionId}请求生成Body,threadId:{Thread.CurrentThread.ManagedThreadId}");
        var data = message.GetShapeData();
        var bodyId = _app.physicsWorld.CreateAndAdd(
            data,
            message.position,
            message.rotation,
            message.motionType,
            message.objectLayer,
            message.activation);
        // _app.physicsWorld.body2Owner[bodyId] = connectionId;
        Log.Information($"生成成功:{bodyId},threadId:{Thread.CurrentThread.ManagedThreadId}");
    }


    private void OnCmdBodyState(in int connectionid, in CmdBodyState message)
    {
        Log.Information($"客户端{connectionid}请求更新Body:{message.entityId}");

        BodyID bodyId = new BodyID(message.entityId);

        if (message.position != null)
        {
            _app.physicsWorld.physicsSystem.BodyInterface.SetPosition(bodyId, message.position.Value,
                (JoltPhysicsSharp.Activation)message.activation);
        }

        if (message.rotation != null)
        {
            _app.physicsWorld.physicsSystem.BodyInterface.SetRotation(bodyId, message.rotation.Value,
                (JoltPhysicsSharp.Activation)message.activation);
        }

        if (message.linearVelocity != null)
        {
            _app.physicsWorld.physicsSystem.BodyInterface.SetLinearVelocity(bodyId, message.linearVelocity.Value);
        }

        if (message.angularVelocity != null)
        {
            _app.physicsWorld.physicsSystem.BodyInterface.SetAngularVelocity(bodyId, message.angularVelocity.Value);
        }

        var active = _app.physicsWorld.physicsSystem.BodyInterface.IsActive(bodyId);
        if (active == message.isActive) return;
        if (message.isActive)
        {
            _app.physicsWorld.physicsSystem.BodyInterface.ActivateBody(bodyId);
        }
        else
        {
            _app.physicsWorld.physicsSystem.BodyInterface.DeactivateBody(bodyId);
        }
    }

    private void OnCmdDestroy(in int connectionid, in CmdDestroy message)
    {
        Log.Information($"客户端{connectionid}请求销毁Body:{message.entityId}");
        _app.RemoveAndDestroy(message.entityId);
        Log.Information($"销毁Body成功:{message.entityId}");
    }
}
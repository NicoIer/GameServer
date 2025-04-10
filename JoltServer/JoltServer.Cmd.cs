using System.Diagnostics;
using GameCore.Jolt;
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
        _server.messageHandler.Add<CmdSpawnBox>(OnCmdSpawnBox);
        _server.messageHandler.Add<CmdSpawnPlane>(OnCmdSpawnPlane);
        _server.messageHandler.Add<CmdDestroy>(OnCmdDestroy);
        _server.messageHandler.Add<CmdBodyState>(OnCmdBodyState);

        // Rpc


    }


    private void OnCmdBodyState(in int connectionid, in CmdBodyState message)
    {
        Log.Information($"客户端{connectionid}请求更新Body:{message.entityId}");
        if (!_body2Owner.ContainsKey(message.entityId))
        {
            Log.Warning($"客户端{connectionid}请求更新不存在的Body:{message.entityId}");
            return;
        }

        if (_body2Owner[message.entityId] != connectionid)
        {
            Log.Warning($"客户端{connectionid}请求更新不属于自己的Body:{message.entityId}由{_body2Owner[message.entityId]}所有");
            return;
        }

        BodyID bodyId = new BodyID(message.entityId);

        if (message.position != null)
        {
            _app.physicsSystem.BodyInterface.SetPosition(bodyId, message.position.Value,
                (JoltPhysicsSharp.Activation)message.activation);
        }

        if (message.rotation != null)
        {
            _app.physicsSystem.BodyInterface.SetRotation(bodyId, message.rotation.Value,
                (JoltPhysicsSharp.Activation)message.activation);
        }

        if (message.linearVelocity != null)
        {
            _app.physicsSystem.BodyInterface.SetLinearVelocity(bodyId, message.linearVelocity.Value);
        }

        if (message.angularVelocity != null)
        {
            _app.physicsSystem.BodyInterface.SetAngularVelocity(bodyId, message.angularVelocity.Value);
        }

        var active = _app.physicsSystem.BodyInterface.IsActive(bodyId);
        if (active == message.isActive) return;
        if (message.isActive)
        {
            _app.physicsSystem.BodyInterface.ActivateBody(bodyId);
        }
        else
        {
            _app.physicsSystem.BodyInterface.DeactivateBody(bodyId);
        }
    }

    private void OnCmdDestroy(in int connectionid, in CmdDestroy message)
    {
        Log.Information($"客户端{connectionid}请求销毁Body:{message.entityId}");

        if (!_body2Owner.ContainsKey(message.entityId))
        {
            Log.Warning($"客户端{connectionid}请求销毁不存在的Body:{message.entityId}");
            return;
        }

        if (_body2Owner[message.entityId] != connectionid)
        {
            Log.Warning($"客户端{connectionid}请求销毁不属于自己的Body:{message.entityId}由{_body2Owner[message.entityId]}所有");
            return;
        }

        _app.RemoveAndDestroy(message.entityId);
        _body2Owner.Remove(message.entityId);


        Log.Information($"销毁Body成功:{message.entityId}");
    }


    public void OnCmdSpawnPlane(in int connectionid, in CmdSpawnPlane message)
    {
        Log.Information($"客户端{connectionid}请求生成Plane");
        var bodyId = _app.CreatePlane(
            message.position,
            message.rotation,
            message.normal,
            message.distance,
            message.halfExtent,
            (JoltPhysicsSharp.MotionType)message.motionType,
            (ushort)message.objectLayer,
            null,
            (JoltPhysicsSharp.Activation)message.activation
        );
        _body2Owner[bodyId.ID] = connectionid;
        Log.Information($"生成Plane成功:{bodyId}");
    }

    public void OnCmdSpawnBox(in int connectionid, in CmdSpawnBox message)
    {
        Log.Information($"客户端{connectionid}请求生成Box");
        Debug.Assert(float.IsNaN(message.rotation.X) == false);
        Debug.Assert(float.IsNaN(message.rotation.Y) == false);
        Debug.Assert(float.IsNaN(message.rotation.Z) == false);
        Debug.Assert(float.IsNaN(message.rotation.W) == false);


        var bodyId = _app.CreateBox(
            message.halfExtents,
            message.position,
            message.rotation,
            (JoltPhysicsSharp.MotionType)message.motionType,
            (uint)message.objectLayer,
            (JoltPhysicsSharp.Activation)message.activation
        );
        _body2Owner[bodyId.ID] = connectionid;
        Log.Information($"生成Box成功:{bodyId}");
    }
}
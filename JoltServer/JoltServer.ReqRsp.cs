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
    private readonly ReqRspServerCenter _reqRspServerCenter = new ReqRspServerCenter();

    private void HandleReqRsp()
    {
        // Req Rsp
        _server.messageHandler.Add<ReqHead>(OnReqBody);
        _reqRspServerCenter.Register<ReqBodyInfo, RspBodyInfo>(OnReqBodyInfo);
    }

    private void OnReqBody(in int connectionid, in ReqHead message)
    {
        var rsp = _reqRspServerCenter.HandleRequest(connectionid, message);
        _server.Send(connectionid, rsp);
    }

    private void OnReqBodyInfo(in int connectionid, in ReqBodyInfo message, out RspBodyInfo rsp,
        out ErrorCode errorcode, out string errorMsg)
    {
        rsp = default;
        errorMsg = "";
        var bodyId = message.bodyId;
        if (!_app.physicsWorld.body2Owner.TryGetValue(bodyId, out var value))
        {
            errorcode = ErrorCode.InvalidArgument;
            errorMsg = "Invalid body id";
            return;
        }

        if (value != connectionid)
        {
            errorcode = ErrorCode.InvalidArgument;
            errorMsg = "Invalid owner";
            return;
        }

        _app.physicsWorld.physicsSystem.BodyLockInterface.LockRead(message.bodyId, out var @lock);

        if (@lock.Succeeded)
        {
            errorcode = ErrorCode.Success;
            _app.physicsWorld.QueryBody(bodyId, out var data);
            rsp = new RspBodyInfo(bodyId, data);
        }
        else
        {
            errorcode = ErrorCode.InternalError;
            errorMsg = "Internal Error LockRead Failed";
        }

        _app.physicsWorld.physicsSystem.BodyLockInterface.UnlockRead(@lock);
    }
}
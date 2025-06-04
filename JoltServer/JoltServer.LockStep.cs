using System.Diagnostics;
using Cysharp.Threading;
using GameCore.Jolt;
using Network.Server;

namespace JoltServer;

/// <summary>
/// Jolt Server 本地代理 
/// </summary>
public partial class JoltServer
{
    private NetworkServer _networkServer;

    private Dictionary<int, LockStepData> _lockSteps;

    private void HandleLockStep()
    {
        Debug.Assert(_config.lockStep);
        var socket = new TelepathyServerSocket(_config.lockStepPort);
        _networkServer = new NetworkServer(socket);

        _networkServer.AddMsgHandler<LockStepData>(OnLockStepData);

        _lockSteps = new Dictionary<int, LockStepData>();
    }


    private void OnLockStepData(in int connectionId, in LockStepData message)
    {
        _lockSteps[connectionId] = message;
        if (message.frame != currentFrame)
        {
            // ?
        }
    }

    private void LockStep(in JoltApplication.LoopContex ctx)
    {
        // 实现锁步逻辑
        // while (_lockSteps.Count < _server.ConnectionCount)
        // {
        //     // Wait
        //     Thread.Sleep(1);
        // }
    }

    private LogicLooper _lockStepLooper;

    private void StartLockStep()
    {
        _networkServer.Run(false);
        _lockStepLooper = new LogicLooper(_app.targetFPS);
        _lockStepLooper.RegisterActionAsync(((in LogicLooperActionContext ctx) =>
        {
            _networkServer.socket.TickIncoming();
            _networkServer.socket.TickOutgoing();
            return true;
        }));
    }

    private void StopLockStep()
    {
        _networkServer.Stop();
        _lockStepLooper.Dispose();
    }
}
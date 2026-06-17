using Game001.Core.Generated;
using GameServer.Core.Fibers;
using Network;

namespace Game001.Room;

public sealed class Game001RoomFiberModule : IFiberModule
{
    private readonly Game001RoomConnectionRegistry _connections;
    private readonly ReqRspServerCenter _reqRspCenter = new();
    private readonly int _frameIntervalMs;
    private readonly RoomRuntimeState _state;
    private long _nextFrameTimeMs;
    private int _frame;

    public Game001RoomFiberModule(string roomId, Game001RoomConnectionRegistry connections, int roomFrameRate)
    {
        RoomId = roomId;
        _connections = connections;
        _frameIntervalMs = 1000 / roomFrameRate;
        _state = new RoomRuntimeState(roomId);
    }

    public string RoomId { get; }

    public ValueTask OnStartAsync(Fiber fiber, CancellationToken cancellationToken)
    {
        var handlers = new Game001RoomReqRspHandlers(_connections, _state);
        NetworkReqRspInitializer.RegisterAll(_reqRspCenter, handlers);
        fiber.NextWakeTimeMs = Environment.TickCount64 + _frameIntervalMs;
        return ValueTask.CompletedTask;
    }

    public void OnUpdate(FiberUpdateContext context)
    {
        if (_nextFrameTimeMs == 0)
        {
            _nextFrameTimeMs = context.TimeNowMs + _frameIntervalMs;
            context.Fiber.NextWakeTimeMs = _nextFrameTimeMs;
            return;
        }

        while (context.TimeNowMs >= _nextFrameTimeMs)
        {
            _frame++;
            _state.Update(_nextFrameTimeMs, _frame);
            _nextFrameTimeMs += _frameIntervalMs;
        }

        context.Fiber.NextWakeTimeMs = _nextFrameTimeMs;
    }

    public void OnLateUpdate(FiberUpdateContext context)
    {
    }

    public ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public RspHead HandleRequest(int connectionId, ReqHead request)
    {
        try
        {
            return _reqRspCenter.HandleRequest(connectionId, request);
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, Network.ErrorCode.InternalError, "room request failed", default);
        }
    }
}

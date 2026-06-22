using GameServer.Core.Fibers;
using Network;

namespace GameServer.Core.Rooms;

public abstract class RoomFiberModuleBase : IFiberModule
{
    private readonly ReqRspServerCenter _reqRspCenter = new();
    private readonly int _frameIntervalMs;
    private long _nextFrameTimeMs;
    private int _frame;

    protected RoomFiberModuleBase(string roomId, int roomFrameRate)
    {
        RoomId = roomId;
        _frameIntervalMs = 1000 / roomFrameRate;
    }

    public string RoomId { get; }
    public virtual RoomLifecycleState LifecycleState => RoomLifecycleState.Active;
    protected RoomFrameAwaiter FrameAwaiter { get; } = new();

    public ValueTask OnStartAsync(Fiber fiber, CancellationToken cancellationToken)
    {
        RegisterHandlers(_reqRspCenter);
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
            OnRoomUpdate(_nextFrameTimeMs, _frame);
            _nextFrameTimeMs += _frameIntervalMs;
        }

        context.Fiber.NextWakeTimeMs = _nextFrameTimeMs;
    }

    public void OnLateUpdate(FiberUpdateContext context)
    {
    }

    public ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        FrameAwaiter.Cancel();
        OnRoomStopped();
        return ValueTask.CompletedTask;
    }

    public async ValueTask<RspHead> HandleRequestAsync(int connectionId, ReqHead request)
    {
        try
        {
            return await _reqRspCenter.HandleRequestAsync(connectionId, request);
        }
        catch
        {
            return new RspHead(request.index, request.reqHash, 0, ErrorCode.InternalError, "room request failed", default);
        }
    }

    public virtual ValueTask HandleConnectionDisconnectedAsync(int connectionId, RoomConnectionContext context)
    {
        return ValueTask.CompletedTask;
    }

    public virtual bool ShouldCloseRoom(long timeNowMs)
    {
        RoomLifecycleState lifecycleState = LifecycleState;
        return lifecycleState == RoomLifecycleState.Empty ||
               lifecycleState == RoomLifecycleState.Closing ||
               lifecycleState == RoomLifecycleState.Closed;
    }

    public virtual void BeginCloseRoom(long timeNowMs)
    {
    }

    protected abstract void RegisterHandlers(ReqRspServerCenter center);
    protected abstract void OnRoomUpdate(long timeNowMs, int frame);
    protected virtual void OnRoomStopped()
    {
    }
}

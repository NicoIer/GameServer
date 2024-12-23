using Cysharp.Threading;
using UnityToolkit;

namespace FishGame;

public class LoopSystem : ISystem, IOnInit
{
    private LogicLooper _logicLooper;

    public delegate void FrameAction(in TimeSpan elapsedTimeFromPreviousFrame);

    private event FrameAction OnOnUpdate = delegate { };

    public LoopSystem(int frameRate)
    {
        _logicLooper = new LogicLooper(frameRate);
    }


    public void OnInit()
    {
        _logicLooper.RegisterActionAsync(OnUpdate);
    }


    public void Dispose()
    {
        _logicLooper.Dispose();
    }

    public void AddOnUpdate(FrameAction action)
    {
        OnOnUpdate += action;
    }

    public void RemoveOnUpdate(FrameAction action)
    {
        OnOnUpdate -= action;
    }

    private bool OnUpdate(in LogicLooperActionContext ctx)
    {
        var ctxElapsedTimeFromPreviousFrame = ctx.ElapsedTimeFromPreviousFrame;
        OnOnUpdate(in ctxElapsedTimeFromPreviousFrame);
        return true;
    }
}
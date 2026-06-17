using Google.Protobuf;

namespace GameServer.Rpc.Server;

public interface IGrpcMessage
{
    IMessage GetRequest();
    void SetCanceled();
    void SetException(Exception exception);
    void SetResult(IMessage result);
    bool IsCanceled();
}

public sealed class GrpcMessage<TRequest, TResponse> : IGrpcMessage
    where TRequest : class, IMessage
    where TResponse : class, IMessage
{
    public required TRequest Request { get; init; }
    public required TaskCompletionSource<TResponse> Tcs { get; init; }
    public CancellationToken CancellationToken { get; init; }

    public IMessage GetRequest()
    {
        return Request;
    }

    public void SetCanceled()
    {
        Tcs.TrySetCanceled(CancellationToken);
    }

    public void SetException(Exception exception)
    {
        Tcs.TrySetException(exception);
    }

    public void SetResult(IMessage result)
    {
        Tcs.TrySetResult((TResponse)result);
    }

    public bool IsCanceled()
    {
        return CancellationToken.IsCancellationRequested;
    }
}

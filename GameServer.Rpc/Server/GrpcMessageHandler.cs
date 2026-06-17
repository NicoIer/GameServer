using Google.Protobuf;

namespace GameServer.Rpc.Server;

[AttributeUsage(AttributeTargets.Class)]
public sealed class GrpcMessageHandlerAttribute : Attribute
{
    public GrpcMessageHandlerAttribute(Type requestType)
    {
        RequestType = requestType;
    }

    public Type RequestType { get; }
}

public interface IGrpcMessageHandler
{
    Type RequestType { get; }
    void Handle(IGrpcMessage message);
}

public abstract class GrpcMessageHandler<TRequest, TResponse> : IGrpcMessageHandler
    where TRequest : class, IMessage
    where TResponse : class, IMessage
{
    public Type RequestType
    {
        get { return typeof(TRequest); }
    }

    public void Handle(IGrpcMessage message)
    {
        var grpcMessage = (GrpcMessage<TRequest, TResponse>)message;

        try
        {
            TResponse response = Run(grpcMessage.Request);
            grpcMessage.SetResult(response);
        }
        catch (Exception exception)
        {
            grpcMessage.SetException(exception);
        }
    }

    protected abstract TResponse Run(TRequest request);
}

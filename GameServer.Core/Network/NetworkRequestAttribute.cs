namespace GameServer.Core.Network;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class NetworkRequestAttribute : Attribute
{
    public Type ResponseType { get; }

    public NetworkRequestAttribute(Type responseType)
    {
        ResponseType = responseType;
    }
}

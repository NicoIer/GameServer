namespace GameServer.Core.Systems;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = true)]
public sealed class ExecuteBeforeAttribute : Attribute
{
    public Type Type { get; }

    public ExecuteBeforeAttribute(Type type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = true)]
public sealed class ExecuteAfterAttribute : Attribute
{
    public Type Type { get; }

    public ExecuteAfterAttribute(Type type)
    {
        Type = type;
    }
}

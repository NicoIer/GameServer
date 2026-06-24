namespace GameServer.Core.Systems;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class ExecuteBeforeAttribute : Attribute
{
    public Type Type { get; }   
}

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class ExecuteAfterAttribute : Attribute
{
    public Type Type { get; }
}
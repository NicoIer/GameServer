using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Analyzers.Tests;

public sealed class EcsReplicationGeneratorTests
{
    private const string AttributeTypes = """
namespace GameServer.Core.Ecs
{
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    public sealed class EcsReplicatedComponentAttribute : System.Attribute
    {
    }
}
""";

    private const string SharedTypes = """
namespace Friflo.Engine.ECS
{
    public interface IComponent
    {
    }
}

namespace MemoryPack
{
    [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
    public sealed class MemoryPackableAttribute : System.Attribute
    {
    }
}
""";

    [Fact]
    public void GeneratesSerializerForReplicatedComponent()
    {
        MetadataReference attributeReference = CreateMetadataReference("GameServer.Core", AttributeTypes);
        string source = SharedTypes + """
namespace Game001.Core.Ecs
{
    [MemoryPack.MemoryPackable]
    [GameServer.Core.Ecs.EcsReplicatedComponent]
    public partial struct RoomPlayerComponent : Friflo.Engine.ECS.IComponent
    {
        public long Uid;
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source, attributeReference);
        string generated = Assert.Single(result.GeneratedTrees).GetText().ToString();

        Assert.Contains("namespace Game001.Core.Generated;", generated);
        Assert.Contains("public static class EcsReplicationSerializer", generated);
        Assert.Contains("RoomPlayerComponentTypeId", generated);
        Assert.Contains("TrySerializeComponent<TBufferWriter>", generated);
        Assert.Contains("where TBufferWriter : class, global::System.Buffers.IBufferWriter<byte>", generated);
        Assert.Contains("MemoryPackSerializer.Serialize(bufferWriter, component0);", generated);
        Assert.Contains("public static void SerializeAllComponents(global::Friflo.Engine.ECS.Entity entity, global::Network.NetworkBuffer<global::Game001.Core.EcsComponentSnapshot> componentWriter, global::Network.NetworkBuffer payloadWriter, out global::System.ArraySegment<global::Game001.Core.EcsComponentSnapshot> result)", generated);
        Assert.Contains("global::MemoryPack.MemoryPackSerializer.Serialize(payloadWriter, component0);", generated);
        Assert.Contains("Payload = payloadWriter.ToArraySegment(payloadOffset, payloadWriter.Position - payloadOffset),", generated);
        Assert.Contains("public static void CreateFullState(global::Friflo.Engine.ECS.EntityStore store, global::Network.NetworkBuffer<global::Game001.Core.EcsEntitySnapshot> entityWriter, global::Network.NetworkBuffer<global::Game001.Core.EcsComponentSnapshot> componentWriter, global::Network.NetworkBuffer payloadWriter, out global::System.ArraySegment<global::Game001.Core.EcsEntitySnapshot> result)", generated);
        Assert.Contains("dirty.MarkComponentUpdated(entity, RoomPlayerComponentTypeId, value);", generated);
        Assert.Contains("SetReplicatedComponent<T>", generated);
    }

    [Fact]
    public void ReportsNonPartialReplicatedComponent()
    {
        MetadataReference attributeReference = CreateMetadataReference("GameServer.Core", AttributeTypes);
        string source = SharedTypes + """
namespace Game001.Core.Ecs
{
    [MemoryPack.MemoryPackable]
    [GameServer.Core.Ecs.EcsReplicatedComponent]
    public struct RoomPlayerComponent : Friflo.Engine.ECS.IComponent
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source, attributeReference);

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSECS002");
    }

    [Fact]
    public void ReportsReplicatedTypeThatIsNotComponent()
    {
        MetadataReference attributeReference = CreateMetadataReference("GameServer.Core", AttributeTypes);
        string source = SharedTypes + """
namespace Game001.Core.Ecs
{
    [MemoryPack.MemoryPackable]
    [GameServer.Core.Ecs.EcsReplicatedComponent]
    public partial struct RoomPlayerComponent
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source, attributeReference);

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSECS001");
    }

    private static GeneratorDriverRunResult RunGenerator(string source, params MetadataReference[] additionalReferences)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));
        CSharpCompilation compilation = CSharpCompilation.Create(
            "Game001.Core",
            new[] { syntaxTree },
            GetReferences().Concat(additionalReferences),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new EcsReplicationGenerator());
        driver = driver.RunGenerators(compilation);
        return driver.GetRunResult();
    }

    private static MetadataReference CreateMetadataReference(string assemblyName, string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        if (!result.Success)
        {
            string errors = string.Join(Environment.NewLine, result.Diagnostics);
            throw new InvalidOperationException(errors);
        }

        return MetadataReference.CreateFromImage(stream.ToArray());
    }

    private static IReadOnlyList<MetadataReference> GetReferences()
    {
        string? trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (!string.IsNullOrWhiteSpace(trustedPlatformAssemblies))
        {
            return trustedPlatformAssemblies
                .Split(Path.PathSeparator)
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToArray();
        }

        return new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        };
    }
}

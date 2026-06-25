using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Analyzers;

[Generator]
public sealed class EcsReplicationGenerator : IIncrementalGenerator
{
    private const string AttributeName = "GameServer.Core.Ecs.EcsReplicatedComponentAttribute";
    private const string ComponentInterfaceName = "Friflo.Engine.ECS.IComponent";
    private const string MemoryPackableAttributeName = "MemoryPack.MemoryPackableAttribute";

    private static readonly DiagnosticDescriptor InvalidComponentRule = new(
        "GSECS001",
        "Invalid replicated ECS component",
        "Type {0} is marked with EcsReplicatedComponentAttribute and must be a struct implementing Friflo.Engine.ECS.IComponent",
        "EcsReplicationGenerator",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor NonPartialComponentRule = new(
        "GSECS002",
        "Replicated ECS component must be partial",
        "Type {0} is marked with EcsReplicatedComponentAttribute and must be partial",
        "EcsReplicationGenerator",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor MissingMemoryPackRule = new(
        "GSECS003",
        "Replicated ECS component must be MemoryPackable",
        "Type {0} is marked with EcsReplicatedComponentAttribute and must also be MemoryPackable",
        "EcsReplicationGenerator",
        DiagnosticSeverity.Error,
        true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TypeDeclarationSyntax> candidateDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is StructDeclarationSyntax { AttributeLists.Count: > 0 },
            static (syntaxContext, _) => (TypeDeclarationSyntax)syntaxContext.Node);

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<TypeDeclarationSyntax> Declarations)> input =
            context.CompilationProvider.Combine(candidateDeclarations.Collect());

        context.RegisterSourceOutput(input, static (sourceContext, item) =>
        {
            Execute(sourceContext, item.Compilation, item.Declarations);
        });
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<TypeDeclarationSyntax> candidates)
    {
        INamedTypeSymbol? attributeType = compilation.GetTypeByMetadataName(AttributeName);
        INamedTypeSymbol? componentInterfaceType = compilation.GetTypeByMetadataName(ComponentInterfaceName);
        INamedTypeSymbol? memoryPackableAttributeType = compilation.GetTypeByMetadataName(MemoryPackableAttributeName);

        if (attributeType == null || componentInterfaceType == null || memoryPackableAttributeType == null)
        {
            return;
        }

        var components = new List<ReplicatedComponent>();
        var seenTypes = new HashSet<string>(StringComparer.Ordinal);
        foreach (TypeDeclarationSyntax declaration in candidates)
        {
            SemanticModel model = compilation.GetSemanticModel(declaration.SyntaxTree);
            if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol ||
                !SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, compilation.Assembly))
            {
                continue;
            }

            if (!HasAttribute(symbol, attributeType))
            {
                continue;
            }

            if (symbol.TypeKind != TypeKind.Struct || !Implements(symbol, componentInterfaceType))
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidComponentRule, declaration.Identifier.GetLocation(), symbol.ToDisplayString()));
                continue;
            }

            if (!declaration.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
            {
                context.ReportDiagnostic(Diagnostic.Create(NonPartialComponentRule, declaration.Identifier.GetLocation(), symbol.ToDisplayString()));
                continue;
            }

            if (!HasAttribute(symbol, memoryPackableAttributeType))
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingMemoryPackRule, declaration.Identifier.GetLocation(), symbol.ToDisplayString()));
                continue;
            }

            string typeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (!seenTypes.Add(typeName))
            {
                continue;
            }

            components.Add(new ReplicatedComponent(symbol, GetStableHashCode16(GetRuntimeFullName(symbol))));
        }

        if (components.Count == 0)
        {
            return;
        }

        components.Sort(static (left, right) => string.CompareOrdinal(left.TypeName, right.TypeName));

        string rootNamespace = compilation.AssemblyName ?? "EcsReplication";
        string generatedNamespace = $"{rootNamespace}.Generated";
        string source = Generate(generatedNamespace, components);
        context.AddSource("EcsReplicationSerializer.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string Generate(string generatedNamespace, List<ReplicatedComponent> components)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine($"namespace {generatedNamespace};");
        sb.AppendLine();
        sb.AppendLine("public static class EcsReplicationSerializer");
        sb.AppendLine("{");
        foreach (ReplicatedComponent component in components)
        {
            sb.Append("    public const ushort ");
            sb.Append(component.ConstantName);
            sb.Append(" = ");
            sb.Append(component.ComponentTypeId);
            sb.AppendLine(";");
        }
        sb.AppendLine();

        GenerateTryGetComponentTypeId(sb, components);
        GenerateTrySerializeComponent(sb, components);
        GenerateSerializeAllComponents(sb, components);
        GenerateCreateFullState(sb);
        GenerateSetReplicatedComponent(sb, components);

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void GenerateTryGetComponentTypeId(StringBuilder sb, List<ReplicatedComponent> components)
    {
        sb.AppendLine("    public static bool TryGetComponentTypeId(global::System.Type type, out ushort componentTypeId)");
        sb.AppendLine("    {");
        foreach (ReplicatedComponent component in components)
        {
            sb.Append("        if (type == typeof(");
            sb.Append(component.TypeName);
            sb.AppendLine("))");
            sb.AppendLine("        {");
            sb.Append("            componentTypeId = ");
            sb.Append(component.ConstantName);
            sb.AppendLine(";");
            sb.AppendLine("            return true;");
            sb.AppendLine("        }");
        }
        sb.AppendLine();
        sb.AppendLine("        componentTypeId = 0;");
        sb.AppendLine("        return false;");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void GenerateTrySerializeComponent(StringBuilder sb, List<ReplicatedComponent> components)
    {
        sb.AppendLine("    public static bool TrySerializeComponent<TBufferWriter>(global::Friflo.Engine.ECS.Entity entity, ushort componentTypeId, TBufferWriter bufferWriter)");
        sb.AppendLine("        where TBufferWriter : class, global::System.Buffers.IBufferWriter<byte>");
        sb.AppendLine("    {");
        sb.AppendLine("        switch (componentTypeId)");
        sb.AppendLine("        {");
        for (int i = 0; i < components.Count; i++)
        {
            ReplicatedComponent component = components[i];
            sb.Append("            case ");
            sb.Append(component.ConstantName);
            sb.AppendLine(":");
            sb.Append("                if (entity.TryGetComponent(out ");
            sb.Append(component.TypeName);
            sb.Append(" component");
            sb.Append(i);
            sb.AppendLine("))");
            sb.AppendLine("                {");
            sb.Append("                    global::MemoryPack.MemoryPackSerializer.Serialize(bufferWriter, component");
            sb.Append(i);
            sb.AppendLine(");");
            sb.AppendLine("                    return true;");
            sb.AppendLine("                }");
            sb.AppendLine("                break;");
        }
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return false;");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void GenerateSerializeAllComponents(StringBuilder sb, List<ReplicatedComponent> components)
    {
        sb.AppendLine("    public static void SerializeAllComponents(global::Friflo.Engine.ECS.Entity entity, global::Network.NetworkBuffer<global::Game001.Core.EcsComponentSnapshot> componentWriter, global::Network.NetworkBuffer payloadWriter, out global::System.ArraySegment<global::Game001.Core.EcsComponentSnapshot> result)");
        sb.AppendLine("    {");
        sb.AppendLine("        int componentOffset = componentWriter.Position;");
        for (int i = 0; i < components.Count; i++)
        {
            ReplicatedComponent component = components[i];
            sb.Append("        if (entity.TryGetComponent(out ");
            sb.Append(component.TypeName);
            sb.Append(" component");
            sb.Append(i);
            sb.AppendLine("))");
            sb.AppendLine("        {");
            sb.AppendLine("            int payloadOffset = payloadWriter.Position;");
            sb.Append("            global::MemoryPack.MemoryPackSerializer.Serialize(payloadWriter, component");
            sb.Append(i);
            sb.AppendLine(");");
            sb.AppendLine("            componentWriter.Write(new global::Game001.Core.EcsComponentSnapshot");
            sb.AppendLine("            {");
            sb.Append("                ComponentTypeId = ");
            sb.Append(component.ConstantName);
            sb.AppendLine(",");
            sb.AppendLine("                Payload = payloadWriter.ToArraySegment(payloadOffset, payloadWriter.Position - payloadOffset),");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
        }
        sb.AppendLine();
        sb.AppendLine("        result = componentWriter.ToArraySegment(componentOffset, componentWriter.Position - componentOffset);");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void GenerateCreateFullState(StringBuilder sb)
    {
        sb.AppendLine("    public static void CreateFullState(global::Friflo.Engine.ECS.EntityStore store, global::Network.NetworkBuffer<global::Game001.Core.EcsEntitySnapshot> entityWriter, global::Network.NetworkBuffer<global::Game001.Core.EcsComponentSnapshot> componentWriter, global::Network.NetworkBuffer payloadWriter, out global::System.ArraySegment<global::Game001.Core.EcsEntitySnapshot> result)");
        sb.AppendLine("    {");
        sb.AppendLine("        entityWriter.Reset();");
        sb.AppendLine("        componentWriter.Reset();");
        sb.AppendLine("        payloadWriter.Reset();");
        sb.AppendLine("        foreach (global::Friflo.Engine.ECS.Entity entity in store.Entities)");
        sb.AppendLine("        {");
        sb.AppendLine("            SerializeAllComponents(entity, componentWriter, payloadWriter, out global::System.ArraySegment<global::Game001.Core.EcsComponentSnapshot> components);");
        sb.AppendLine("            if (components.Count == 0)");
        sb.AppendLine("            {");
        sb.AppendLine("                continue;");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            entityWriter.Write(new global::Game001.Core.EcsEntitySnapshot");
        sb.AppendLine("            {");
        sb.AppendLine("                EntityId = (int)entity.Pid,");
        sb.AppendLine("                Components = components,");
        sb.AppendLine("            });");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        result = entityWriter.ToArraySegment();");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void GenerateSetReplicatedComponent(StringBuilder sb, List<ReplicatedComponent> components)
    {
        sb.AppendLine("    public static void SetReplicatedComponent<T>(global::Friflo.Engine.ECS.Entity entity, T component, global::Game001.Core.Ecs.EcsDirtyTracker dirty)");
        sb.AppendLine("        where T : struct, global::Friflo.Engine.ECS.IComponent");
        sb.AppendLine("    {");
        sb.AppendLine("        entity.AddComponent(component);");
        sb.AppendLine("        global::System.Type type = typeof(T);");
        foreach (ReplicatedComponent component in components)
        {
            sb.Append("        if (type == typeof(");
            sb.Append(component.TypeName);
            sb.AppendLine("))");
            sb.AppendLine("        {");
            sb.Append("            var value = (");
            sb.Append(component.TypeName);
            sb.AppendLine(")(object)component;");
            sb.Append("            dirty.MarkComponentUpdated(entity, ");
            sb.Append(component.ConstantName);
            sb.AppendLine(", value);");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static bool HasAttribute(INamedTypeSymbol symbol, INamedTypeSymbol attributeType)
    {
        foreach (AttributeData attribute in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
            {
                return true;
            }
        }

        return false;
    }

    private static bool Implements(ITypeSymbol type, INamedTypeSymbol interfaceType)
    {
        foreach (INamedTypeSymbol item in type.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(item, interfaceType))
            {
                return true;
            }
        }

        return false;
    }

    private static string GetRuntimeFullName(INamedTypeSymbol symbol)
    {
        string typeName = symbol.MetadataName;
        INamedTypeSymbol? containingType = symbol.ContainingType;
        while (containingType != null)
        {
            typeName = containingType.MetadataName + "+" + typeName;
            containingType = containingType.ContainingType;
        }

        if (symbol.ContainingNamespace is { IsGlobalNamespace: false } containingNamespace)
        {
            typeName = containingNamespace.ToDisplayString() + "." + typeName;
        }

        return typeName;
    }

    private static ushort GetStableHashCode16(string text)
    {
        unchecked
        {
            uint hash = 0x811c9dc5;
            const uint prime = 0x1000193;

            for (int i = 0; i < text.Length; ++i)
            {
                byte value = (byte)text[i];
                hash ^= value;
                hash *= prime;
            }

            return (ushort)(((int)hash >> 16) ^ (int)hash);
        }
    }

    private readonly struct ReplicatedComponent
    {
        public INamedTypeSymbol Symbol { get; }
        public ushort ComponentTypeId { get; }
        public string TypeName => Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        public string ConstantName => Symbol.Name + "TypeId";

        public ReplicatedComponent(INamedTypeSymbol symbol, ushort componentTypeId)
        {
            Symbol = symbol;
            ComponentTypeId = componentTypeId;
        }
    }
}

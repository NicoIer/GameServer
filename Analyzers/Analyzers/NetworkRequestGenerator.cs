using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Analyzers;

[Generator]
public sealed class NetworkRequestGenerator : IIncrementalGenerator
{
    private const string NetworkRequestAttributeName = "GameServer.Core.Network.NetworkRequestAttribute";
    private const string NetworkReqName = "Network.INetworkReq";
    private const string NetworkRspName = "Network.INetworkRsp";
    private const string ReqRspServerCenterName = "Network.ReqRspServerCenter";
    private const string ErrorCodeName = "Network.ErrorCode";

    private static readonly DiagnosticDescriptor MissingTypesRule = new(
        "GSRPC001",
        "Missing network request generator dependencies",
        "Cannot resolve {0}",
        "NetworkRequestGenerator",
        DiagnosticSeverity.Warning,
        true);

    private static readonly DiagnosticDescriptor InvalidResponseRule = new(
        "GSRPC002",
        "Invalid network response type",
        "Request {0} must reference a response type that implements Network.INetworkRsp",
        "NetworkRequestGenerator",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor DuplicateRequestRule = new(
        "GSRPC003",
        "Duplicate network request",
        "Request type {0} is registered more than once",
        "NetworkRequestGenerator",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor InvalidRequestRule = new(
        "GSRPC005",
        "Invalid network request type",
        "Type {0} is marked with NetworkRequestAttribute and must implement Network.INetworkReq",
        "NetworkRequestGenerator",
        DiagnosticSeverity.Error,
        true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TypeDeclarationSyntax> candidateDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is TypeDeclarationSyntax declaration &&
                (declaration.AttributeLists.Count > 0 || IsReqRspHandlerDeclaration(declaration)),
            static (syntaxContext, _) => (TypeDeclarationSyntax)syntaxContext.Node);

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<TypeDeclarationSyntax> Declarations)> input =
            context.CompilationProvider.Combine(candidateDeclarations.Collect());

        context.RegisterSourceOutput(input, static (sourceContext, item) =>
        {
            Execute(sourceContext, item.Compilation, item.Declarations);
        });
    }

    private static bool IsReqRspHandlerDeclaration(TypeDeclarationSyntax declaration)
    {
        return declaration is ClassDeclarationSyntax classDeclaration &&
            classDeclaration.Identifier.ValueText.EndsWith("ReqRspHandlers", StringComparison.Ordinal);
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<TypeDeclarationSyntax> candidates)
    {
        INamedTypeSymbol? attributeType = compilation.GetTypeByMetadataName(NetworkRequestAttributeName);
        INamedTypeSymbol? reqType = compilation.GetTypeByMetadataName(NetworkReqName);
        INamedTypeSymbol? rspType = compilation.GetTypeByMetadataName(NetworkRspName);
        INamedTypeSymbol? centerType = compilation.GetTypeByMetadataName(ReqRspServerCenterName);
        INamedTypeSymbol? errorCodeType = compilation.GetTypeByMetadataName(ErrorCodeName);

        if (attributeType == null || reqType == null || rspType == null || centerType == null || errorCodeType == null)
        {
            string missing = string.Join(", ", new[]
            {
                attributeType == null ? NetworkRequestAttributeName : null,
                reqType == null ? NetworkReqName : null,
                rspType == null ? NetworkRspName : null,
                centerType == null ? ReqRspServerCenterName : null,
                errorCodeType == null ? ErrorCodeName : null,
            }.Where(x => x != null));
            context.ReportDiagnostic(Diagnostic.Create(MissingTypesRule, Location.None, missing));
            return;
        }

        List<RequestPair> pairs = GetCurrentPairs(context, compilation, candidates, attributeType, reqType, rspType);
        if (pairs.Count > 0)
        {
            string rootNamespace = compilation.AssemblyName ?? "NetworkRequests";
            string generatedNamespace = $"{rootNamespace}.Generated";
            string source = Generate(generatedNamespace, pairs);
            context.AddSource("NetworkReqRspInitializer.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        List<RequestPair> handlerPairs = GetReferencedPairs(compilation, attributeType, reqType, rspType);
        handlerPairs.AddRange(pairs);
        if (handlerPairs.Count == 0)
        {
            return;
        }

        GenerateHandlerBridges(context, compilation, candidates, handlerPairs);
    }

    private static List<RequestPair> GetCurrentPairs(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<TypeDeclarationSyntax> candidates,
        INamedTypeSymbol attributeType,
        INamedTypeSymbol reqType,
        INamedTypeSymbol rspType)
    {
        var pairs = new List<RequestPair>();
        var seen = new Dictionary<string, INamedTypeSymbol>(StringComparer.Ordinal);

        foreach (TypeDeclarationSyntax declaration in candidates)
        {
            SemanticModel model = compilation.GetSemanticModel(declaration.SyntaxTree);
            if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol requestSymbol ||
                !SymbolEqualityComparer.Default.Equals(requestSymbol.ContainingAssembly, compilation.Assembly))
            {
                continue;
            }

            AttributeData? attribute = requestSymbol.GetAttributes()
                .FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeType));
            if (attribute == null)
            {
                continue;
            }

            if (!Implements(requestSymbol, reqType))
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidRequestRule, declaration.Identifier.GetLocation(), requestSymbol.ToDisplayString()));
                continue;
            }

            if (attribute.ConstructorArguments.Length == 0 ||
                attribute.ConstructorArguments[0].Value is not INamedTypeSymbol responseSymbol ||
                !Implements(responseSymbol, rspType))
            {
                context.ReportDiagnostic(Diagnostic.Create(InvalidResponseRule, declaration.Identifier.GetLocation(), requestSymbol.ToDisplayString()));
                continue;
            }

            string requestName = requestSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (seen.TryGetValue(requestName, out INamedTypeSymbol existingRequest))
            {
                if (!SymbolEqualityComparer.Default.Equals(existingRequest, requestSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DuplicateRequestRule, declaration.Identifier.GetLocation(), requestName));
                }

                continue;
            }

            seen.Add(requestName, requestSymbol);
            pairs.Add(new RequestPair(requestSymbol, responseSymbol));
        }

        return pairs;
    }

    private static string Generate(string generatedNamespace, List<RequestPair> pairs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine($"namespace {generatedNamespace};");
        sb.AppendLine();
        sb.AppendLine("public interface INetworkReqRspHandlers");
        sb.AppendLine("{");
        foreach (RequestPair pair in pairs)
        {
            sb.Append("    global::System.Threading.Tasks.ValueTask<(");
            sb.Append(TypeName(pair.Response));
            sb.Append(" rsp, global::Network.ErrorCode errorCode, string errorMsg)> Handle(int connectionId, ");
            sb.Append(TypeName(pair.Request));
            sb.Append(" req);");
            sb.AppendLine();
        }
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("public static class NetworkReqRspInitializer");
        sb.AppendLine("{");
        sb.AppendLine("    public static void RegisterAll(global::Network.ReqRspServerCenter center, INetworkReqRspHandlers handlers)");
        sb.AppendLine("    {");
        foreach (RequestPair pair in pairs)
        {
            sb.Append("        center.Register<");
            sb.Append(TypeName(pair.Request));
            sb.Append(", ");
            sb.Append(TypeName(pair.Response));
            sb.AppendLine(">(handlers.Handle);");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void GenerateHandlerBridges(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<TypeDeclarationSyntax> candidates,
        List<RequestPair> pairs)
    {
        var handlerSymbols = new List<INamedTypeSymbol>();
        var seenHandlers = new HashSet<string>(StringComparer.Ordinal);
        foreach (TypeDeclarationSyntax declaration in candidates)
        {
            SemanticModel model = compilation.GetSemanticModel(declaration.SyntaxTree);
            if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol ||
                symbol.TypeKind != TypeKind.Class ||
                !SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, compilation.Assembly) ||
                !symbol.Name.EndsWith("ReqRspHandlers", StringComparison.Ordinal))
            {
                continue;
            }

            string handlerName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (seenHandlers.Add(handlerName))
            {
                handlerSymbols.Add(symbol);
            }
        }

        if (handlerSymbols.Count == 0)
        {
            return;
        }

        foreach (INamedTypeSymbol handlerSymbol in handlerSymbols)
        {
            string handlerNamespace = handlerSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : handlerSymbol.ContainingNamespace.ToDisplayString();
            var nestedHandlerNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (INamedTypeSymbol nestedType in handlerSymbol.GetTypeMembers())
            {
                if (nestedType.TypeKind == TypeKind.Class)
                {
                    nestedHandlerNames.Add(nestedType.Name);
                }
            }

            foreach (RequestPair pair in pairs)
            {
                string nestedName = GetHandlerNestedName(pair.Request.Name);
                if (!nestedHandlerNames.Contains(nestedName))
                {
                    continue;
                }

                string interfaceNamespace = GetGeneratedNamespace(pair.Request.ContainingAssembly.Name);
                var sb = new StringBuilder();
                sb.AppendLine("// <auto-generated />");
                sb.AppendLine("#nullable enable");
                sb.AppendLine();
                if (handlerNamespace.Length > 0)
                {
                    sb.AppendLine($"namespace {handlerNamespace};");
                    sb.AppendLine();
                }

                sb.Append("public sealed partial class ");
                sb.Append(handlerSymbol.Name);
                sb.Append(" : global::");
                sb.Append(interfaceNamespace);
                sb.AppendLine(".INetworkReqRspHandlers");
                sb.AppendLine("{");
                sb.Append("    public global::System.Threading.Tasks.ValueTask<(");
                sb.Append(TypeName(pair.Response));
                sb.Append(" rsp, global::Network.ErrorCode errorCode, string errorMsg)> Handle(int connectionId, ");
                sb.Append(TypeName(pair.Request));
                sb.AppendLine(" req)");
                sb.AppendLine("    {");
                sb.Append("        return ");
                sb.Append(nestedName);
                sb.AppendLine(".Handle(this, connectionId, req);");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.Append("    public static partial class ");
                sb.Append(nestedName);
                sb.AppendLine();
                sb.AppendLine("    {");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                string hintName = $"Handlers/{handlerSymbol.Name}.{nestedName}.g.cs";
                context.AddSource(hintName, SourceText.From(sb.ToString(), Encoding.UTF8));
            }
        }
    }

    private static List<RequestPair> GetReferencedPairs(
        Compilation compilation,
        INamedTypeSymbol attributeType,
        INamedTypeSymbol reqType,
        INamedTypeSymbol rspType)
    {
        var pairs = new List<RequestPair>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (MetadataReference reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assembly)
            {
                continue;
            }

            CollectNamespacePairs(assembly.GlobalNamespace, attributeType, reqType, rspType, pairs, seen);
        }

        return pairs;
    }

    private static void CollectNamespacePairs(
        INamespaceSymbol namespaceSymbol,
        INamedTypeSymbol attributeType,
        INamedTypeSymbol reqType,
        INamedTypeSymbol rspType,
        List<RequestPair> pairs,
        HashSet<string> seen)
    {
        foreach (INamedTypeSymbol type in namespaceSymbol.GetTypeMembers())
        {
            CollectTypePairs(type, attributeType, reqType, rspType, pairs, seen);
        }

        foreach (INamespaceSymbol child in namespaceSymbol.GetNamespaceMembers())
        {
            CollectNamespacePairs(child, attributeType, reqType, rspType, pairs, seen);
        }
    }

    private static void CollectTypePairs(
        INamedTypeSymbol type,
        INamedTypeSymbol attributeType,
        INamedTypeSymbol reqType,
        INamedTypeSymbol rspType,
        List<RequestPair> pairs,
        HashSet<string> seen)
    {
        AttributeData? attribute = type.GetAttributes()
            .FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeType));
        if (attribute != null &&
            Implements(type, reqType) &&
            attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is INamedTypeSymbol responseSymbol &&
            Implements(responseSymbol, rspType))
        {
            string requestName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (seen.Add(requestName))
            {
                pairs.Add(new RequestPair(type, responseSymbol));
            }
        }

        foreach (INamedTypeSymbol nestedType in type.GetTypeMembers())
        {
            CollectTypePairs(nestedType, attributeType, reqType, rspType, pairs, seen);
        }
    }

    private static string GetGeneratedNamespace(string assemblyName)
    {
        return $"{assemblyName}.Generated";
    }

    private static string GetHandlerNestedName(string requestTypeName)
    {
        if (requestTypeName.EndsWith("Req", StringComparison.Ordinal))
        {
            return requestTypeName.Substring(0, requestTypeName.Length - 3) + "ReqRsp";
        }

        return requestTypeName + "ReqRsp";
    }

    private static bool Implements(INamedTypeSymbol type, INamedTypeSymbol interfaceType)
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

    private static string TypeName(ITypeSymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    private readonly struct RequestPair
    {
        public INamedTypeSymbol Request { get; }
        public INamedTypeSymbol Response { get; }

        public RequestPair(INamedTypeSymbol request, INamedTypeSymbol response)
        {
            Request = request;
            Response = response;
        }
    }
}

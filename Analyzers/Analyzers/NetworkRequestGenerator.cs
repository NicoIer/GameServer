using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Analyzers;

[Generator]
public sealed class NetworkRequestGenerator : ISourceGenerator
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

    private static readonly DiagnosticDescriptor MissingAttributeRule = new(
        "GSRPC004",
        "Missing NetworkRequest attribute",
        "Request {0} implements Network.INetworkReq and must be marked with NetworkRequestAttribute",
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

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new Receiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not Receiver receiver)
        {
            return;
        }

        INamedTypeSymbol? attributeType = context.Compilation.GetTypeByMetadataName(NetworkRequestAttributeName);
        INamedTypeSymbol? reqType = context.Compilation.GetTypeByMetadataName(NetworkReqName);
        INamedTypeSymbol? rspType = context.Compilation.GetTypeByMetadataName(NetworkRspName);
        INamedTypeSymbol? centerType = context.Compilation.GetTypeByMetadataName(ReqRspServerCenterName);
        INamedTypeSymbol? errorCodeType = context.Compilation.GetTypeByMetadataName(ErrorCodeName);

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

        var pairs = new List<RequestPair>();
        var seen = new Dictionary<string, INamedTypeSymbol>(StringComparer.Ordinal);

        foreach (TypeDeclarationSyntax declaration in receiver.Candidates)
        {
            SemanticModel model = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
            if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol requestSymbol)
            {
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(requestSymbol.ContainingAssembly, context.Compilation.Assembly))
            {
                continue;
            }

            AttributeData? attribute = requestSymbol.GetAttributes()
                .FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeType));

            if (!Implements(requestSymbol, reqType))
            {
                if (attribute != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(InvalidRequestRule, declaration.Identifier.GetLocation(), requestSymbol.ToDisplayString()));
                }

                continue;
            }

            if (attribute == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingAttributeRule, declaration.Identifier.GetLocation(), requestSymbol.ToDisplayString()));
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

        if (pairs.Count > 0)
        {
            string rootNamespace = context.Compilation.AssemblyName ?? "NetworkRequests";
            string generatedNamespace = $"{rootNamespace}.Generated";
            string source = Generate(generatedNamespace, pairs);
            context.AddSource("NetworkReqRspInitializer.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        List<RequestPair> handlerPairs = GetReferencedPairs(context.Compilation, attributeType, reqType, rspType);
        handlerPairs.AddRange(pairs);
        if (handlerPairs.Count == 0)
        {
            return;
        }

        string handlerSource = GenerateHandlerBridges(context, receiver.Candidates, handlerPairs);
        if (handlerSource.Length > 0)
        {
            context.AddSource("NetworkReqRspHandlerBridges.g.cs", SourceText.From(handlerSource, Encoding.UTF8));
        }
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

    private static string GenerateHandlerBridges(GeneratorExecutionContext context, List<TypeDeclarationSyntax> candidates, List<RequestPair> pairs)
    {
        var handlerSymbols = new List<INamedTypeSymbol>();
        foreach (TypeDeclarationSyntax declaration in candidates)
        {
            SemanticModel model = context.Compilation.GetSemanticModel(declaration.SyntaxTree);
            if (model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol ||
                symbol.TypeKind != TypeKind.Class ||
                !SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, context.Compilation.Assembly) ||
                !symbol.Name.EndsWith("ReqRspHandlers", StringComparison.Ordinal))
            {
                continue;
            }

            handlerSymbols.Add(symbol);
        }

        if (handlerSymbols.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        foreach (INamedTypeSymbol handlerSymbol in handlerSymbols)
        {
            string handlerNamespace = handlerSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : handlerSymbol.ContainingNamespace.ToDisplayString();
            if (handlerNamespace.Length > 0)
            {
                sb.AppendLine($"namespace {handlerNamespace};");
                sb.AppendLine();
            }

            string interfaceNamespace = GetGeneratedNamespace(pairs[0].Request.ContainingAssembly.Name);
            sb.Append("public sealed partial class ");
            sb.Append(handlerSymbol.Name);
            sb.Append(" : global::");
            sb.Append(interfaceNamespace);
            sb.AppendLine(".INetworkReqRspHandlers");
            sb.AppendLine("{");
            foreach (RequestPair pair in pairs)
            {
                string nestedName = GetHandlerNestedName(pair.Request.Name);
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
            }
            sb.AppendLine("}");
            sb.AppendLine();
        }

        return sb.ToString();
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

    private sealed class Receiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> Candidates { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclaration)
            {
                Candidates.Add(typeDeclaration);
            }
        }
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

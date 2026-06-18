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

    private static readonly DiagnosticDescriptor DuplicateResponseRule = new(
        "GSRPC008",
        "Duplicate network response",
        "Request {0} references response {1}, which is already used by request {2}",
        "NetworkRequestGenerator",
        DiagnosticSeverity.Error,
        true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<TypeDeclarationSyntax> candidateDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
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
        if (pairs.Count == 0)
        {
            return;
        }

        string rootNamespace = compilation.AssemblyName ?? "NetworkRequests";
        string generatedNamespace = $"{rootNamespace}.Generated";
        string handlerInterfaceName = GetHandlerInterfaceName(rootNamespace);
        string source = Generate(generatedNamespace, handlerInterfaceName, pairs);
        context.AddSource("NetworkReqRspInitializer.g.cs", SourceText.From(source, Encoding.UTF8));
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
        var seenRequests = new Dictionary<string, INamedTypeSymbol>(StringComparer.Ordinal);
        var seenResponses = new Dictionary<string, INamedTypeSymbol>(StringComparer.Ordinal);

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
            if (seenRequests.TryGetValue(requestName, out INamedTypeSymbol existingRequest))
            {
                if (!SymbolEqualityComparer.Default.Equals(existingRequest, requestSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DuplicateRequestRule, declaration.Identifier.GetLocation(), requestName));
                }

                continue;
            }

            string responseName = responseSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (seenResponses.TryGetValue(responseName, out INamedTypeSymbol existingResponseRequest) &&
                !SymbolEqualityComparer.Default.Equals(existingResponseRequest, requestSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DuplicateResponseRule,
                    declaration.Identifier.GetLocation(),
                    requestSymbol.ToDisplayString(),
                    responseSymbol.ToDisplayString(),
                    existingResponseRequest.ToDisplayString()));
                continue;
            }

            seenRequests.Add(requestName, requestSymbol);
            seenResponses.Add(responseName, requestSymbol);
            pairs.Add(new RequestPair(requestSymbol, responseSymbol));
        }

        return pairs;
    }

    private static string Generate(string generatedNamespace, string handlerInterfaceName, List<RequestPair> pairs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine($"namespace {generatedNamespace};");
        sb.AppendLine();
        sb.Append("public interface ");
        sb.AppendLine(handlerInterfaceName);
        sb.AppendLine("{");
        foreach (RequestPair pair in pairs)
        {
            sb.Append("    global::System.Threading.Tasks.ValueTask<(");
            sb.Append(TypeName(pair.Response));
            sb.Append(" rsp, global::Network.ErrorCode errorCode, string errorMsg)> ");
            sb.Append(GetHandlerMethodName(pair.Request.Name));
            sb.Append("(int connectionId, ");
            sb.Append(TypeName(pair.Request));
            sb.Append(" req);");
            sb.AppendLine();
        }
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("public static class NetworkReqRspInitializer");
        sb.AppendLine("{");
        sb.Append("    public static void RegisterAll(global::Network.ReqRspServerCenter center, ");
        sb.Append(handlerInterfaceName);
        sb.AppendLine(" handlers)");
        sb.AppendLine("    {");
        foreach (RequestPair pair in pairs)
        {
            sb.Append("        center.Register<");
            sb.Append(TypeName(pair.Request));
            sb.Append(", ");
            sb.Append(TypeName(pair.Response));
            sb.Append(">(handlers.");
            sb.Append(GetHandlerMethodName(pair.Request.Name));
            sb.AppendLine(");");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetHandlerInterfaceName(string assemblyName)
    {
        string gameName = assemblyName.EndsWith(".Core", StringComparison.Ordinal)
            ? assemblyName.Substring(0, assemblyName.Length - ".Core".Length)
            : assemblyName;

        int dotIndex = gameName.LastIndexOf('.');
        if (dotIndex >= 0)
        {
            gameName = gameName.Substring(dotIndex + 1);
        }

        return $"I{gameName}Handler";
    }

    private static string GetHandlerMethodName(string requestTypeName)
    {
        string methodName = requestTypeName.EndsWith("Req", StringComparison.Ordinal)
            ? requestTypeName.Substring(0, requestTypeName.Length - 3)
            : requestTypeName;

        return $"Handle{methodName}";
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

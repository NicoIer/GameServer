using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Game001.CodeGenerator;

public sealed class RoomHandlerGenerationStep : ICodeGenerationStep
{
    public string Name => "room-handler";

    public CodeGenerationResult Execute(
        CodeGenerationContext context,
        CSharpSourceCatalog coreSources)
    {
        SortedDictionary<string, RoomHandlerInfo> requestHandlers = CollectRequests(coreSources);
        HashSet<string> implementedMethods = CollectImplementedMethods(context.RoomHandlersDirectory);
        int createdCount = 0;
        int skippedCount = 0;

        foreach (KeyValuePair<string, RoomHandlerInfo> item in requestHandlers)
        {
            if (implementedMethods.Contains(item.Key))
            {
                skippedCount++;
                continue;
            }

            string handlerPath = GetNewHandlerPath(
                context.RoomHandlersDirectory,
                item.Key,
                item.Value.BaseName);
            GeneratedFileWriter.WriteNew(
                handlerPath,
                GenerateHandler(item.Key, item.Value.RequestType, item.Value.ResponseType));
            createdCount++;
        }

        return new CodeGenerationResult(createdCount, 0, skippedCount);
    }

    private static SortedDictionary<string, RoomHandlerInfo> CollectRequests(
        CSharpSourceCatalog coreSources)
    {
        var result = new SortedDictionary<string, RoomHandlerInfo>(StringComparer.Ordinal);
        foreach (CSharpSourceFile sourceFile in coreSources.Files)
        {
            foreach (TypeDeclarationSyntax declaration in sourceFile.Root
                         .DescendantNodes()
                         .OfType<TypeDeclarationSyntax>())
            {
                AttributeSyntax? attribute = CSharpSyntax.FindAttribute(declaration, "NetworkRequest");
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax typeOf)
                {
                    throw new InvalidOperationException(
                        $"NetworkRequest must declare typeof(response): {declaration.Identifier.ValueText}");
                }

                string baseName = declaration.Identifier.ValueText.EndsWith("Req", StringComparison.Ordinal)
                    ? declaration.Identifier.ValueText.Substring(0, declaration.Identifier.ValueText.Length - 3)
                    : declaration.Identifier.ValueText;
                string methodName = "Handle" + baseName;
                result.Add(methodName, new RoomHandlerInfo(
                    baseName,
                    CSharpSyntax.GetTypeName(declaration),
                    CSharpSyntax.GetReferencedTypeName(declaration, typeOf.Type)));
            }
        }

        return result;
    }

    private static HashSet<string> CollectImplementedMethods(string handlersDirectory)
    {
        Directory.CreateDirectory(handlersDirectory);
        CSharpSourceCatalog handlerSources = CSharpSourceCatalog.Load(handlersDirectory);
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (CSharpSourceFile sourceFile in handlerSources.Files)
        {
            foreach (ClassDeclarationSyntax declaration in sourceFile.Root
                         .DescendantNodes()
                         .OfType<ClassDeclarationSyntax>())
            {
                if (declaration.Identifier.ValueText != "Game001RoomReqRspHandlers")
                {
                    continue;
                }

                foreach (MethodDeclarationSyntax method in declaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    result.Add(method.Identifier.ValueText);
                }
            }
        }

        return result;
    }

    private static string GetNewHandlerPath(
        string handlersDirectory,
        string methodName,
        string baseName)
    {
        string handlerPath = Path.Combine(
            handlersDirectory,
            "Game001RoomReqRspHandlers." + baseName + ".cs");
        if (!File.Exists(handlerPath))
        {
            return handlerPath;
        }

        handlerPath = Path.Combine(
            handlersDirectory,
            "Game001RoomReqRspHandlers." + baseName + ".Generated.cs");
        if (File.Exists(handlerPath))
        {
            throw new InvalidOperationException($"generated handler still misses {methodName}: {handlerPath}");
        }

        return handlerPath;
    }

    private static string GenerateHandler(
        string methodName,
        string requestType,
        string responseType)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// Generated once by Game001.CodeGenerator. Replace the NotSupported body with room logic.");
        builder.AppendLine("using NetworkErrorCode = Network.ErrorCode;");
        builder.AppendLine();
        builder.AppendLine("namespace Game001.Room;");
        builder.AppendLine();
        builder.AppendLine("public sealed partial class Game001RoomReqRspHandlers");
        builder.AppendLine("{");
        builder.Append("    public global::System.Threading.Tasks.ValueTask<(global::");
        builder.Append(responseType);
        builder.AppendLine(" rsp, NetworkErrorCode errorCode, string errorMsg)>");
        builder.Append("        ");
        builder.Append(methodName);
        builder.AppendLine("(");
        builder.AppendLine("            int connectionId,");
        builder.Append("            global::");
        builder.Append(requestType);
        builder.AppendLine(" req)");
        builder.AppendLine("    {");
        builder.Append("        global::");
        builder.Append(responseType);
        builder.AppendLine(" rsp = default!;");
        builder.Append("        return new global::System.Threading.Tasks.ValueTask<(global::");
        builder.Append(responseType);
        builder.AppendLine(", NetworkErrorCode, string)>(");
        builder.Append("            (rsp, NetworkErrorCode.NotSupported, \"");
        builder.Append(methodName);
        builder.AppendLine(" is not implemented\"));");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private readonly struct RoomHandlerInfo
    {
        public string BaseName { get; }
        public string RequestType { get; }
        public string ResponseType { get; }

        public RoomHandlerInfo(string baseName, string requestType, string responseType)
        {
            BaseName = baseName;
            RequestType = requestType;
            ResponseType = responseType;
        }
    }
}

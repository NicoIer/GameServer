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
        RoomMessageCatalog messages = RoomMessageCatalog.Collect(coreSources);
        Dictionary<string, HashSet<string>> implementedMethods = CollectImplementedMethods(context.RoomDirectory);
        int createdCount = 0;
        int skippedCount = 0;

        foreach (RoomRequestInfo request in messages.Requests)
        {
            string handlerType = request.Kind == "Worker"
                ? "Game001RoomWorker"
                : "Game001RoomReqRspHandlers";
            if (HasMethod(implementedMethods, handlerType, request.HandlerName))
            {
                skippedCount++;
                continue;
            }

            string handlerPath = GetNewHandlerPath(
                context.RoomHandlersDirectory,
                handlerType,
                request.BaseName,
                request.HandlerName);
            string content = request.Kind == "Worker"
                ? GenerateWorkerHandler(request)
                : GenerateRoomRequestHandler(request);
            GeneratedFileWriter.WriteNew(handlerPath, content);
            createdCount++;
        }

        foreach (RoomCommandInfo command in messages.Commands)
        {
            const string handlerType = "Game001RoomCommandHandlers";
            if (HasMethod(implementedMethods, handlerType, command.HandlerName))
            {
                skippedCount++;
                continue;
            }

            string handlerPath = GetNewHandlerPath(
                context.RoomHandlersDirectory,
                handlerType,
                command.BaseName,
                command.HandlerName);
            GeneratedFileWriter.WriteNew(handlerPath, GenerateCommandHandler(command));
            createdCount++;
        }

        return new CodeGenerationResult(createdCount, 0, skippedCount);
    }

    private static Dictionary<string, HashSet<string>> CollectImplementedMethods(string roomDirectory)
    {
        CSharpSourceCatalog roomSources = CSharpSourceCatalog.Load(roomDirectory);
        var result = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        foreach (CSharpSourceFile sourceFile in roomSources.Files)
        {
            foreach (ClassDeclarationSyntax declaration in sourceFile.Root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string typeName = declaration.Identifier.ValueText;
                if (!result.TryGetValue(typeName, out HashSet<string>? methods))
                {
                    methods = new HashSet<string>(StringComparer.Ordinal);
                    result.Add(typeName, methods);
                }

                foreach (MethodDeclarationSyntax method in declaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    methods.Add(method.Identifier.ValueText);
                }
            }
        }

        return result;
    }

    private static bool HasMethod(
        Dictionary<string, HashSet<string>> implementedMethods,
        string handlerType,
        string methodName)
    {
        return implementedMethods.TryGetValue(handlerType, out HashSet<string>? methods) &&
               methods.Contains(methodName);
    }

    private static string GetNewHandlerPath(
        string handlersDirectory,
        string handlerType,
        string baseName,
        string methodName)
    {
        Directory.CreateDirectory(handlersDirectory);
        string handlerPath = Path.Combine(handlersDirectory, handlerType + "." + baseName + ".cs");
        if (!File.Exists(handlerPath))
        {
            return handlerPath;
        }

        handlerPath = Path.Combine(handlersDirectory, handlerType + "." + baseName + ".Generated.cs");
        if (File.Exists(handlerPath))
        {
            throw new InvalidOperationException($"generated handler still misses {methodName}: {handlerPath}");
        }

        return handlerPath;
    }

    private static string GenerateRoomRequestHandler(RoomRequestInfo request)
    {
        var builder = new StringBuilder();
        AppendRequestHeader(builder, "Game001RoomReqRspHandlers");
        AppendRequestMethod(builder, request, false);
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string GenerateWorkerHandler(RoomRequestInfo request)
    {
        var builder = new StringBuilder();
        AppendRequestHeader(builder, "Game001RoomWorker");
        AppendRequestMethod(builder, request, true);
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void AppendRequestHeader(StringBuilder builder, string handlerType)
    {
        builder.AppendLine("// Generated once by Game001.CodeGenerator. Replace the NotSupported body with room logic.");
        builder.AppendLine("using NetworkErrorCode = Network.ErrorCode;");
        builder.AppendLine();
        builder.AppendLine("namespace Game001.Room;");
        builder.AppendLine();
        builder.Append("public sealed partial class ");
        builder.AppendLine(handlerType);
        builder.AppendLine("{");
    }

    private static void AppendRequestMethod(
        StringBuilder builder,
        RoomRequestInfo request,
        bool includeContext)
    {
        builder.Append("    public global::System.Threading.Tasks.ValueTask<(global::");
        builder.Append(request.ResponseType);
        builder.AppendLine(" rsp, NetworkErrorCode errorCode, string errorMsg)>");
        builder.Append("        ");
        builder.Append(request.HandlerName);
        builder.AppendLine("(");
        builder.AppendLine("            int connectionId,");
        builder.Append("            global::");
        builder.Append(request.RequestType);
        builder.Append(includeContext ? "," : ")");
        builder.AppendLine();
        if (includeContext)
        {
            builder.AppendLine("            global::GameServer.Core.Rooms.RoomConnectionContext context)");
        }

        builder.AppendLine("    {");
        builder.Append("        global::");
        builder.Append(request.ResponseType);
        builder.AppendLine(" rsp = default!;");
        builder.Append("        return new global::System.Threading.Tasks.ValueTask<(global::");
        builder.Append(request.ResponseType);
        builder.AppendLine(", NetworkErrorCode, string)>(");
        builder.Append("            (rsp, NetworkErrorCode.NotSupported, \"");
        builder.Append(request.HandlerName);
        builder.AppendLine(" is not implemented\"));");
        builder.AppendLine("    }");
    }

    private static string GenerateCommandHandler(RoomCommandInfo command)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// Generated once by Game001.CodeGenerator. Replace the NotSupported body with room logic.");
        builder.AppendLine("namespace Game001.Room;");
        builder.AppendLine();
        builder.AppendLine("public sealed partial class Game001RoomCommandHandlers");
        builder.AppendLine("{");
        builder.Append("    public void ");
        builder.Append(command.HandlerName);
        builder.AppendLine("(");
        builder.AppendLine("        int connectionId,");
        builder.Append("        global::");
        builder.Append(command.CommandType);
        builder.AppendLine(" command)");
        builder.AppendLine("    {");
        builder.Append("        throw new global::System.NotSupportedException(\"");
        builder.Append(command.HandlerName);
        builder.AppendLine(" is not implemented\");");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }
}

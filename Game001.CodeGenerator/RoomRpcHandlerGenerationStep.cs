using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Game001.CodeGenerator;

public sealed class RoomRpcHandlerGenerationStep : ICodeGenerationStep
{
    public string Name => "room-rpc-handler";

    public CodeGenerationResult Execute(CodeGenerationContext context, CSharpSourceCatalog coreSources)
    {
        RoomRpcCatalog catalog = RoomRpcCatalog.Collect(coreSources);
        HashSet<string> serverMethods = CollectMethods(context.RoomDirectory, "Game001RoomServerRpcHandlers");
        HashSet<string> clientMethods = Directory.Exists(context.UnityClientGameDirectory)
            ? CollectMethods(context.UnityClientGameDirectory, "Game001ClientRpcHandlers")
            : new HashSet<string>(StringComparer.Ordinal);
        int created = 0;
        int skipped = 0;

        foreach (RoomRpcContractInfo contract in catalog.Contracts)
        {
            foreach (RoomRpcMethodInfo method in contract.Methods)
            {
                bool server = method.Direction == RoomRpcDirection.Server;
                HashSet<string> methods = server ? serverMethods : clientMethods;
                if (methods.Contains(method.HandlerName))
                {
                    skipped++;
                    continue;
                }

                string directory = server
                    ? context.RoomHandlersDirectory
                    : context.UnityClientRpcHandlersDirectory;
                string handlerType = server
                    ? "Game001RoomServerRpcHandlers"
                    : "Game001ClientRpcHandlers";
                string path = GetNewHandlerPath(directory, handlerType, method.HandlerName);
                GeneratedFileWriter.WriteNew(path, GenerateHandler(handlerType, method, server));
                created++;
            }
        }

        return new CodeGenerationResult(created, 0, skipped);
    }

    private static HashSet<string> CollectMethods(string directory, string typeName)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        CSharpSourceCatalog sources = CSharpSourceCatalog.Load(directory);
        foreach (CSharpSourceFile source in sources.Files)
        {
            foreach (ClassDeclarationSyntax type in source.Root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                if (type.Identifier.ValueText != typeName)
                {
                    continue;
                }

                foreach (MethodDeclarationSyntax method in type.Members.OfType<MethodDeclarationSyntax>())
                {
                    result.Add(method.Identifier.ValueText);
                }
            }
        }
        return result;
    }

    private static string GenerateHandler(string handlerType, RoomRpcMethodInfo method, bool server)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// Generated once by Game001.CodeGenerator. Replace the NotSupported body with RPC logic.");
        builder.Append("namespace Game001");
        builder.AppendLine(server ? ".Room" : string.Empty);
        builder.AppendLine("{");
        builder.AppendLine();
        builder.Append("    public sealed partial class ");
        builder.AppendLine(handlerType);
        builder.AppendLine("    {");
        builder.Append("        public void ");
        builder.Append(method.HandlerName);
        builder.Append("(");
        if (server)
        {
            builder.Append("global::GameServer.Core.Rooms.RoomServerRpcContext context");
        }
        else
        {
            builder.Append("int entityId");
        }
        foreach (RoomRpcParameterInfo parameter in method.Parameters)
        {
            builder.Append(", ");
            builder.Append(parameter.TypeName);
            builder.Append(" ");
            builder.Append(parameter.Name);
        }
        builder.AppendLine(")");
        builder.AppendLine("        {");
        builder.Append("            throw new global::System.NotSupportedException(\"");
        builder.Append(method.HandlerName);
        builder.AppendLine(" is not implemented\");");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string GetNewHandlerPath(string directory, string handlerType, string methodName)
    {
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, handlerType + "." + methodName + ".cs");
        if (!File.Exists(path))
        {
            return path;
        }

        path = Path.Combine(directory, handlerType + "." + methodName + ".Generated.cs");
        if (File.Exists(path))
        {
            throw new InvalidOperationException($"generated RPC handler still misses {methodName}: {path}");
        }

        return path;
    }
}

using System.Text;

namespace Game001.CodeGenerator;

public interface ICodeGenerationStep
{
    string Name { get; }
    CodeGenerationResult Execute(CodeGenerationContext context, CSharpSourceCatalog coreSources);
}

public readonly record struct CodeGenerationResult(int Created, int Updated, int Skipped);

public enum GeneratedFileChange
{
    Unchanged,
    Created,
    Updated,
}

public sealed class CodeGenerationContext
{
    public string RepositoryRoot { get; }
    public string CoreDirectory { get; }
    public string UnityRuntimeDirectory { get; }
    public string GeneratedRuntimeDirectory { get; }
    public string RoomDirectory { get; }
    public string RoomHandlersDirectory { get; }
    public string RoomGeneratedDirectory { get; }
    public string UnityClientRoot { get; }
    public string UnityClientGameDirectory { get; }
    public string UnityClientGeneratedDirectory { get; }
    public string UnityClientRpcHandlersDirectory { get; }

    private CodeGenerationContext(string repositoryRoot, string unityClientRoot)
    {
        RepositoryRoot = repositoryRoot;
        CoreDirectory = Path.Combine(repositoryRoot, "Game001.Core");
        UnityRuntimeDirectory = Path.Combine(CoreDirectory, "UnityPackage", "Runtime");
        GeneratedRuntimeDirectory = Path.Combine(UnityRuntimeDirectory, "Generated");
        RoomDirectory = Path.Combine(repositoryRoot, "Game001.Room");
        RoomHandlersDirectory = Path.Combine(RoomDirectory, "Handlers");
        RoomGeneratedDirectory = Path.Combine(RoomDirectory, "Generated");
        UnityClientRoot = unityClientRoot;
        UnityClientGameDirectory = Path.Combine(unityClientRoot, "Assets", "Games", "Game001");
        UnityClientGeneratedDirectory = Path.Combine(UnityClientGameDirectory, "Generated");
        UnityClientRpcHandlersDirectory = Path.Combine(
            UnityClientGameDirectory,
            "RpcHandlers");
    }

    public static CodeGenerationContext Create(IReadOnlyList<string> args)
    {
        string startPath = GetStartPath(args);
        DirectoryInfo? directory = new DirectoryInfo(Path.GetFullPath(startPath));
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "GameServer.slnx")))
            {
                string unityClientRoot = GetOption(args, "--unity-root") ??
                                         Path.Combine(directory.Parent!.FullName, "Game001");
                return new CodeGenerationContext(
                    directory.FullName,
                    Path.GetFullPath(unityClientRoot));
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"cannot find GameServer.slnx from {startPath}");
    }

    private static string GetStartPath(IReadOnlyList<string> args)
    {
        for (int i = 0; i < args.Count; i++)
        {
            if (!args[i].StartsWith("--", StringComparison.Ordinal))
            {
                return args[i];
            }

            i++;
        }

        return Directory.GetCurrentDirectory();
    }

    private static string? GetOption(IReadOnlyList<string> args, string option)
    {
        for (int i = 0; i < args.Count - 1; i++)
        {
            if (args[i] == option)
            {
                return args[i + 1];
            }
        }

        return null;
    }
}

public static class GeneratedFileWriter
{
    private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

    public static GeneratedFileChange WriteIfChanged(string path, string content)
    {
        string normalizedContent = content.ReplaceLineEndings("\n");
        if (File.Exists(path))
        {
            if (File.ReadAllText(path) == normalizedContent)
            {
                return GeneratedFileChange.Unchanged;
            }

            File.WriteAllText(path, normalizedContent, Utf8NoBom);
            return GeneratedFileChange.Updated;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, normalizedContent, Utf8NoBom);
        return GeneratedFileChange.Created;
    }

    public static void WriteNew(string path, string content)
    {
        if (File.Exists(path))
        {
            throw new InvalidOperationException($"generated file already exists: {path}");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content.ReplaceLineEndings("\n"), Utf8NoBom);
    }
}

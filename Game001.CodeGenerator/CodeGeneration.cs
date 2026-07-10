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
    public string RoomHandlersDirectory { get; }

    private CodeGenerationContext(string repositoryRoot)
    {
        RepositoryRoot = repositoryRoot;
        CoreDirectory = Path.Combine(repositoryRoot, "Game001.Core");
        UnityRuntimeDirectory = Path.Combine(CoreDirectory, "UnityPackage", "Runtime");
        GeneratedRuntimeDirectory = Path.Combine(UnityRuntimeDirectory, "Generated");
        RoomHandlersDirectory = Path.Combine(repositoryRoot, "Game001.Room", "Handlers");
    }

    public static CodeGenerationContext Create(IReadOnlyList<string> args)
    {
        string startPath = args.Count > 0 ? args[0] : Directory.GetCurrentDirectory();
        DirectoryInfo? directory = new DirectoryInfo(Path.GetFullPath(startPath));
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "GameServer.slnx")))
            {
                return new CodeGenerationContext(directory.FullName);
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"cannot find GameServer.slnx from {startPath}");
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

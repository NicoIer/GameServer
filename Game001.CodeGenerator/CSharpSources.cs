using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Game001.CodeGenerator;

public sealed class CSharpSourceFile
{
    public string FullPath { get; }
    public string RelativePath { get; }
    public CompilationUnitSyntax Root { get; }

    public CSharpSourceFile(string fullPath, string relativePath, CompilationUnitSyntax root)
    {
        FullPath = fullPath;
        RelativePath = relativePath;
        Root = root;
    }
}

public sealed class CSharpSourceCatalog
{
    private readonly List<CSharpSourceFile> _files;

    private CSharpSourceCatalog(List<CSharpSourceFile> files)
    {
        _files = files;
    }

    public IReadOnlyList<CSharpSourceFile> Files => _files;

    public static CSharpSourceCatalog Load(string rootDirectory)
    {
        var files = new List<CSharpSourceFile>();
        foreach (string sourcePath in Directory
                     .EnumerateFiles(rootDirectory, "*.cs", SearchOption.AllDirectories)
                     .OrderBy(path => path, StringComparer.Ordinal))
        {
            string relativePath = Path.GetRelativePath(rootDirectory, sourcePath);
            if (IsBuildOutput(relativePath))
            {
                continue;
            }

            string source = File.ReadAllText(sourcePath);
            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(source).GetCompilationUnitRoot();
            files.Add(new CSharpSourceFile(sourcePath, relativePath, root));
        }

        return new CSharpSourceCatalog(files);
    }

    private static bool IsBuildOutput(string relativePath)
    {
        string firstDirectory = relativePath.Split(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar)[0];
        return firstDirectory == "bin" || firstDirectory == "obj";
    }
}

public static class CSharpSyntax
{
    public static AttributeSyntax? FindAttribute(TypeDeclarationSyntax declaration, string attributeName)
    {
        foreach (AttributeSyntax attribute in declaration.AttributeLists.SelectMany(list => list.Attributes))
        {
            string currentName = attribute.Name.ToString();
            int separatorIndex = currentName.LastIndexOf('.');
            if (separatorIndex >= 0)
            {
                currentName = currentName.Substring(separatorIndex + 1);
            }

            if (currentName == attributeName || currentName == attributeName + "Attribute")
            {
                return attribute;
            }
        }

        return null;
    }

    public static string GetTypeName(TypeDeclarationSyntax declaration)
    {
        var parts = new List<string>();
        foreach (BaseNamespaceDeclarationSyntax item in declaration.Ancestors()
                     .OfType<BaseNamespaceDeclarationSyntax>()
                     .Reverse())
        {
            parts.Add(item.Name.ToString());
        }

        foreach (TypeDeclarationSyntax item in declaration.Ancestors()
                     .OfType<TypeDeclarationSyntax>()
                     .Reverse())
        {
            parts.Add(item.Identifier.ValueText);
        }

        parts.Add(declaration.Identifier.ValueText);
        return string.Join(".", parts);
    }

    public static string GetReferencedTypeName(
        TypeDeclarationSyntax declaration,
        TypeSyntax referencedType)
    {
        string typeName = referencedType.ToString();
        if (typeName.StartsWith("global::", StringComparison.Ordinal))
        {
            return typeName.Substring("global::".Length);
        }

        if (referencedType is IdentifierNameSyntax or GenericNameSyntax)
        {
            string namespaceName = GetNamespaceName(declaration);
            if (namespaceName.Length != 0)
            {
                return namespaceName + "." + typeName;
            }
        }

        return typeName;
    }

    private static string GetNamespaceName(TypeDeclarationSyntax declaration)
    {
        return string.Join(
            ".",
            declaration.Ancestors()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .Reverse()
                .Select(item => item.Name.ToString()));
    }
}

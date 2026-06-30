using System.Text.Json;

namespace GameServer.Config;

public static class LubanConfigLoader
{
    public static TTables Load<TTables>(string dataDirectory, Func<Func<string, JsonElement>, TTables> factory)
    {
        string resolvedDirectory = ResolveDataDirectory(dataDirectory);
        return factory(tableName => LoadTable(resolvedDirectory, tableName));
    }

    public static JsonElement LoadTable(string dataDirectory, string tableName)
    {
        string path = Path.Combine(dataDirectory, tableName + ".json");
        using FileStream stream = File.OpenRead(path);
        using JsonDocument document = JsonDocument.Parse(stream);
        return document.RootElement.Clone();
    }

    public static string ResolveDataDirectory(string dataDirectory)
    {
        if (Path.IsPathRooted(dataDirectory) || Directory.Exists(dataDirectory))
        {
            return dataDirectory;
        }

        string outputPath = Path.Combine(AppContext.BaseDirectory, dataDirectory);
        if (Directory.Exists(outputPath))
        {
            return outputPath;
        }

        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory != null)
        {
            string candidate = Path.Combine(directory.FullName, dataDirectory);
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return dataDirectory;
    }
}

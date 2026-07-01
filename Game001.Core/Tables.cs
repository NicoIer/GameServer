using System.Text.Json;

namespace cfg;

public partial class Tables
{
    public static readonly Tables current;

    static Tables()
    {
        current = new Tables(LoadTable);
    }

    private static JsonElement LoadTable(string tableName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Generated", "Luban", tableName + ".json");
        using FileStream stream = File.OpenRead(path);
        using JsonDocument document = JsonDocument.Parse(stream);
        return document.RootElement.Clone();
    }
}

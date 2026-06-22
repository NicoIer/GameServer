using System.Text.Json;

namespace GameServer.Core.Startup;

public sealed class ServerStartupConfig
{
    public CenterStartupConfig Center { get; set; } = new();
    public GateStartupConfig Gate { get; set; } = new();
}

public sealed class CenterStartupConfig
{
    public int Port { get; set; } = 5001;
    public string Address { get; set; } = "http://127.0.0.1:5001";
}

public sealed class GateStartupConfig
{
    public int Port { get; set; } = 5002;
}

public class RoomServerStartupConfig
{
    public string GameId { get; set; } = string.Empty;
    public string Target { get; set; } = "room-worker";
    public string RouteId { get; set; } = "worker-001";
    public int GrpcPort { get; set; } = 5101;
    public string GrpcAddress { get; set; } = string.Empty;
    public string DirectProtocol { get; set; } = "Tcp";
    public int DirectTcpPort { get; set; } = 6101;
    public string DirectAddress { get; set; } = "127.0.0.1:6101";
    public int NetworkTickMs { get; set; } = 1;
}

public static class ServerStartupConfigLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static ServerStartupConfig Load(string[] args)
    {
        return Load<ServerStartupConfig>(args);
    }

    public static TConfig Load<TConfig>(string[] args)
        where TConfig : new()
    {
        string path = ResolveConfigPath(FindConfigPath(args));
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TConfig>(json, SerializerOptions) ?? new TConfig();
    }

    private static string FindConfigPath(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--config")
            {
                return args[i + 1];
            }
        }

        string? value = Environment.GetEnvironmentVariable("GAME_SERVER_CONFIG");
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return "appsettings.json";
    }

    private static string ResolveConfigPath(string path)
    {
        if (File.Exists(path) || Path.IsPathRooted(path))
        {
            return path;
        }

        string outputPath = Path.Combine(AppContext.BaseDirectory, path);
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        return path;
    }
}

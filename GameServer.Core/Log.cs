using Serilog;

namespace GameServer.Core;

public static class Log
{
    private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    private static readonly object Sync = new();
    private static Serilog.Core.Logger _logger = null!;

    static Log()
    {
        Configure();
    }

    public static void Configure(string logDirectory = "logs")
    {
        lock (Sync)
        {
            Directory.CreateDirectory(logDirectory);

            _logger?.Dispose();
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: OutputTemplate)
                .WriteTo.File(
                    Path.Combine(logDirectory, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: OutputTemplate)
                .CreateLogger();
        }
    }

    public static void Info(string message)
    {
        _logger.Information("{Message}", message);
    }

    public static void Warning(string message)
    {
        _logger.Warning("{Message}", message);
    }

    public static void Error(string message)
    {
        _logger.Error("{Message}", message);
    }

    public static void Error(Exception exception)
    {
        _logger.Error(exception, "{Message}", exception.Message);
    }

    public static void Error(Exception exception, string message)
    {
        _logger.Error(exception, "{Message}", message);
    }
}

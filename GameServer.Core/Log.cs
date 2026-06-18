using Serilog;
using UnityToolkit;

namespace GameServer.Core;

public static class Log
{
    private const string LogTagPropertyName = "LogTag";
    private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] [{LogTag}] {Message:lj}{NewLine}{Exception}";
    private static ILogger _logger = Serilog.Log.Logger;

    internal static ILogger SerilogLogger => Serilog.Log.Logger;

    static Log()
    {
        Configure();
    }

    public static void Configure(string logDirectory = "logs")
    {
        Directory.CreateDirectory(logDirectory);

        Serilog.Log.Logger = new LoggerConfiguration()
#if RELEASE
            .MinimumLevel.Information()
#else
            .MinimumLevel.Debug()
#endif
            .Enrich.FromLogContext()
            .WriteTo.Async(writeTo => writeTo.Console(outputTemplate: OutputTemplate))
            .WriteTo.Async(writeTo => writeTo.File(
                Path.Combine(logDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: OutputTemplate))
            .WriteTo.Logger(loggerConfiguration => loggerConfiguration
                .Filter.ByIncludingOnly(logEvent => logEvent.Properties.ContainsKey(LogTagPropertyName))
                .WriteTo.Async(writeTo => writeTo.Map(
                    LogTagPropertyName,
                    "Other",
                    (tag, mappedWriteTo) => mappedWriteTo.File(
                        Path.Combine(logDirectory, $"{tag}-.txt"),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: OutputTemplate))))
            .CreateLogger();

        _logger = Serilog.Log.Logger.ForContext("SourceContext", "GameServer.Core.Log");
        RedirectToolkitLog();
    }

    private static void RedirectToolkitLog()
    {
        ToolkitLog.writeLog = false;
        ToolkitLog.infoAction = Info;
        ToolkitLog.warningAction = Warning;
        ToolkitLog.errorAction = Error;
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

    public static void Info(string tag, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Information("{Message}", message);
    }

    public static void Warning(string tag, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Warning("{Message}", message);
    }

    public static void Error(string tag, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Error("{Message}", message);
    }

    public static void Error(string tag, Exception exception)
    {
        _logger.ForContext(LogTagPropertyName, tag).Error(exception, "{Message}", exception.Message);
    }

    public static void Error(string tag, Exception exception, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Error(exception, "{Message}", message);
    }
}

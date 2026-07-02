using System.Threading;
using Serilog;
using UnityToolkit;

namespace GameServer.Core;

public static class Log
{
    private const string LogTagPropertyName = "LogTag";
    private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] [{LogTag}] {Message:lj}{NewLine}{Exception}";
    private static readonly AsyncLocal<string?> MessagePrefix = new();
    private static ILogger _logger = Serilog.Log.Logger;

    internal static ILogger SerilogLogger => Serilog.Log.Logger;

    public readonly struct MessagePrefixScope : IDisposable
    {
        private readonly string? _previousPrefix;

        internal MessagePrefixScope(string prefix)
        {
            _previousPrefix = MessagePrefix.Value;
            MessagePrefix.Value = string.IsNullOrEmpty(_previousPrefix) ? prefix : $"{_previousPrefix} {prefix}";
        }

        public void Dispose()
        {
            MessagePrefix.Value = _previousPrefix;
        }
    }

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
        _logger.Information("{Message}", AddMessagePrefix(message));
    }

    public static void Warning(string message)
    {
        _logger.Warning("{Message}", AddMessagePrefix(message));
    }

    public static void Error(string message)
    {
        _logger.Error("{Message}", AddMessagePrefix(message));
    }

    public static void Error(Exception exception)
    {
        _logger.Error(exception, "{Message}", AddMessagePrefix(exception.Message));
    }

    public static void Error(Exception exception, string message)
    {
        _logger.Error(exception, "{Message}", AddMessagePrefix(message));
    }

    public static void Info(string tag, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Information("{Message}", AddMessagePrefix(message));
    }

    public static void Warning(string tag, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Warning("{Message}", AddMessagePrefix(message));
    }

    public static void Error(string tag, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Error("{Message}", AddMessagePrefix(message));
    }

    public static void Error(string tag, Exception exception)
    {
        _logger.ForContext(LogTagPropertyName, tag).Error(exception, "{Message}", AddMessagePrefix(exception.Message));
    }

    public static void Error(string tag, Exception exception, string message)
    {
        _logger.ForContext(LogTagPropertyName, tag).Error(exception, "{Message}", AddMessagePrefix(message));
    }

    public static MessagePrefixScope PushMessagePrefix(string prefix)
    {
        return new MessagePrefixScope(prefix);
    }

    private static string AddMessagePrefix(string message)
    {
        string? prefix = MessagePrefix.Value;
        if (string.IsNullOrEmpty(prefix))
        {
            return message;
        }

        return $"{prefix} {message}";
    }
}

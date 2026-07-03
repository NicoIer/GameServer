using System.Threading;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using UnityToolkit;

namespace GameServer.Core;

public enum LogColor
{
    Default,
    Red,
    Yellow,
    Green,
    Blue,
    Cyan,
    Magenta,
    White,
    Gray,
}

public static class Log
{
    private const string LogTagPropertyName = "LogTag";
    private const string LogColorPropertyName = "LogColor";
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
            .WriteTo.Async(writeTo => writeTo.Map(
                LogColorPropertyName,
                LogColor.Default,
                (color, mappedWriteTo) => mappedWriteTo.Console(
                    outputTemplate: OutputTemplate,
                    theme: GetConsoleTheme(color)),
                sinkMapCountLimit: 16))
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
        Info(message, LogColor.Default);
    }

    public static void Info(string message, LogColor color)
    {
        WithColor(_logger, color).Information("{Message}", AddMessagePrefix(message));
    }

    public static void Warning(string message)
    {
        Warning(message, LogColor.Default);
    }

    public static void Warning(string message, LogColor color)
    {
        WithColor(_logger, color).Warning("{Message}", AddMessagePrefix(message));
    }

    public static void Error(string message)
    {
        Error(message, LogColor.Default);
    }

    public static void Error(string message, LogColor color)
    {
        WithColor(_logger, color).Error("{Message}", AddMessagePrefix(message));
    }

    public static void Error(Exception exception)
    {
        Error(exception, LogColor.Default);
    }

    public static void Error(Exception exception, LogColor color)
    {
        WithColor(_logger, color).Error(exception, "{Message}", AddMessagePrefix(exception.Message));
    }

    public static void Error(Exception exception, string message)
    {
        Error(exception, message, LogColor.Default);
    }

    public static void Error(Exception exception, string message, LogColor color)
    {
        WithColor(_logger, color).Error(exception, "{Message}", AddMessagePrefix(message));
    }

    public static void Info(string tag, string message)
    {
        Info(tag, message, LogColor.Default);
    }

    public static void Info(string tag, string message, LogColor color)
    {
        WithColor(_logger.ForContext(LogTagPropertyName, tag), color).Information("{Message}", AddMessagePrefix(message));
    }

    public static void Warning(string tag, string message)
    {
        Warning(tag, message, LogColor.Default);
    }

    public static void Warning(string tag, string message, LogColor color)
    {
        WithColor(_logger.ForContext(LogTagPropertyName, tag), color).Warning("{Message}", AddMessagePrefix(message));
    }

    public static void Error(string tag, string message)
    {
        Error(tag, message, LogColor.Default);
    }

    public static void Error(string tag, string message, LogColor color)
    {
        WithColor(_logger.ForContext(LogTagPropertyName, tag), color).Error("{Message}", AddMessagePrefix(message));
    }

    public static void Error(string tag, Exception exception)
    {
        Error(tag, exception, LogColor.Default);
    }

    public static void Error(string tag, Exception exception, LogColor color)
    {
        WithColor(_logger.ForContext(LogTagPropertyName, tag), color).Error(exception, "{Message}", AddMessagePrefix(exception.Message));
    }

    public static void Error(string tag, Exception exception, string message)
    {
        Error(tag, exception, message, LogColor.Default);
    }

    public static void Error(string tag, Exception exception, string message, LogColor color)
    {
        WithColor(_logger.ForContext(LogTagPropertyName, tag), color).Error(exception, "{Message}", AddMessagePrefix(message));
    }

    public static MessagePrefixScope PushMessagePrefix(string prefix)
    {
        return new MessagePrefixScope(prefix);
    }

    private static ILogger WithColor(ILogger logger, LogColor color)
    {
        if (color == LogColor.Default)
        {
            return logger;
        }

        return logger.ForContext(LogColorPropertyName, color);
    }

    private static ConsoleTheme GetConsoleTheme(LogColor color)
    {
        return color switch
        {
            LogColor.Red => CreateAnsiTheme("\u001b[31m"),
            LogColor.Yellow => CreateAnsiTheme("\u001b[33m"),
            LogColor.Green => CreateAnsiTheme("\u001b[32m"),
            LogColor.Blue => CreateAnsiTheme("\u001b[34m"),
            LogColor.Cyan => CreateAnsiTheme("\u001b[36m"),
            LogColor.Magenta => CreateAnsiTheme("\u001b[35m"),
            LogColor.White => CreateAnsiTheme("\u001b[37m"),
            LogColor.Gray => CreateAnsiTheme("\u001b[90m"),
            _ => SystemConsoleTheme.Literate,
        };
    }

    private static ConsoleTheme CreateAnsiTheme(string ansi)
    {
        Dictionary<ConsoleThemeStyle, string> styles = new();

        foreach (ConsoleThemeStyle style in Enum.GetValues<ConsoleThemeStyle>())
        {
            styles[style] = ansi;
        }

        return new AnsiConsoleTheme(styles);
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

﻿using System.Text;
using Network;
using Network.Server;
using Serilog;
using Serilog.Events;
using UnityToolkit;

string logPath = $"./log/{DateTime.Now:yyyy-MM-dd}-.txt";
ToolkitLog.writeLog = false; // 取消ToolkitLog的日志写文件\
ToolkitLog.infoAction = Log.Information; // 用Serilog库的Information方法输出日志
ToolkitLog.warningAction = Log.Warning; // 用Serilog库的Warning方法输出日志
ToolkitLog.errorAction = Log.Error; // 用Serilog库的Error方法输出日志


// 使用Serilog库配置日志输出
var loggerConfig = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File(
        logPath,
        restrictedToMinimumLevel: LogEventLevel.Warning, // 日志输出最低级别
        outputTemplate: @"{Timestamp:yyyy-MM-dd HH:mm-ss.fff }[{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day, //日志按天保存
        rollOnFileSizeLimit: true, // 限制单个文件的最大长度
        fileSizeLimitBytes: 10 * 1024 * 1024, // 单个文件最大长度10M
        encoding: Encoding.UTF8, // 文件字符编码
        retainedFileCountLimit: 1024) // 最大保存文件数,超过最大文件数会自动覆盖原有文件
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information); // 控制台输出日志

Log.Logger = loggerConfig.CreateLogger();


string fullLogPath = System.IO.Path.GetFullPath(logPath);
Log.Information("Log file path: {fullLogPath}, logPath: {logPath}", fullLogPath, logPath);


IServerSocket serverSocket = new TelepathyServerSocket(23333);
NetworkServer server = new NetworkServer(serverSocket);

// server.AddMsgHandler((int connectionId, BrodcastMessage msg) =>
// {
//     Log.Information("Receive message from {connectionId}: {msg}", connectionId, msg);
//     server.SendToAll(msg);
// });

await server.Run(true);
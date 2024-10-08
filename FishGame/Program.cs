// See https://aka.ms/new-console-template for more information

using System;
using System.Text;
using FishGame;
using FishGame.Service;
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

//
//
// string path1 = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
// //获取和设置当前目录(该进程从中启动的目录)的完全限定目录
// string path2 = System.Environment.CurrentDirectory;
// //获取应用程序的当前工作目录
// string path3 = System.IO.Directory.GetCurrentDirectory();
// //获取程序的基目录
// string path4 = System.AppDomain.CurrentDomain.BaseDirectory;
// //获取和设置包括该应用程序的目录的名称
// string path5 = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
// //获取启动了应用程序的可执行文件的路径
// StringBuilder str = new StringBuilder();
// str.AppendLine("System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName:" + path1);
// str.AppendLine("System.Environment.CurrentDirectory:" + path2);
// str.AppendLine("System.IO.Directory.GetCurrentDirectory():" + path3);
// str.AppendLine("System.AppDomain.CurrentDomain.BaseDirectory:" + path4);
// str.AppendLine("System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase:" + path5);
// string allPath = str.ToString();
//
// Console.WriteLine(allPath);


Global.Singleton.Add(new GameDatabase());
Global.Singleton.Add(new LoopSystem(60));
Global.Singleton.AddTask(new GameRPC(args));

await Global.Singleton.Run();

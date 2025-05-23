// // See https://aka.ms/new-console-template for more information
// //
using System.Numerics;
using System.Text;
using GameCore.Jolt;
using JoltPhysicsSharp;
using JoltServer;
using Network.Time;
using Serilog;
using Serilog.Events;
using UnityToolkit;
using Activation = JoltPhysicsSharp.Activation;
using MotionType = JoltPhysicsSharp.MotionType;
// //
// //
// var app = new JoltApplication();
// app.AfterPhysicsUpdate += (in JoltApplication.LoopContex ctx) =>
// {
//     for (var i = 0; i < app.bodies.Count; i++)
//     {
//         var id = app.bodies[i];
//         var shape = app.physicsSystem.BodyInterface.GetShape(id);
//         if (shape is not PlaneShape && shape is not BoxShape)
//         {
//             Console.WriteLine("Error Catch");
//         }
//     }
// };
//
//
//
// // Thread.Sleep(1000);
//
// app.CreatePlane(new Vector3(0, 0, 0), Quaternion.Identity, new Vector3(0, 1, 0), 0, 10, MotionType.Static,
//     (uint)ObjectLayers.NonMoving);
//
// // Thread.Sleep(1000);
// for (int i = 0; i < 1000; i++)
// {
//     Thread.Sleep(16);
//     app.CreateBox(new Vector3(1, 1, 1), new Vector3(0, 8, 0), Quaternion.Identity, MotionType.Dynamic,
//         (uint)ObjectLayers.Moving);
//    
// }
//
// app.Run();
// Console.ReadLine();



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

ToolkitLog.infoAction = Log.Information;
ToolkitLog.warningAction = Log.Warning;
ToolkitLog.errorAction = Log.Error;

ShapeDataPacket.RegisterAll();


var app = new JoltApplication();
// var visualDebugger = new JoltRaylibDebugger(1200, 800, "Jolt.Shared Visual Debugger", app.targetFPS);
// app.AddSystem(visualDebugger); 
var joltServer = new JoltServer.JoltServer(60, 24419, 1024, JoltConfig.Default);
app.AddSystem(joltServer);


var clientThread = new Thread(() =>
{
    Thread.Sleep(2000);
    joltServer.OnCmdSpawnPlane(0,new CmdSpawnPlane()
    {
        position = Vector3.Zero,
        rotation = Quaternion.Identity,
        motionType = GameCore.Jolt.MotionType.Static,
        normal = Vector3.UnitY,
        distance = 0,
        halfExtent = 10,
        activation = GameCore.Jolt.Activation.Activate,
        objectLayer = ObjectLayers.NonMoving
    });
    for (int i = 0; i < 10; i++)
    {
        joltServer.OnCmdSpawnBox(0, new CmdSpawnBox()
        {
            halfExtents = Vector3.One,
            position = new Vector3(0, 10, 0),
            rotation = Quaternion.Identity,
            motionType = GameCore.Jolt.MotionType.Static,
            activation = GameCore.Jolt.Activation.Activate,
            objectLayer = ObjectLayers.Moving,
        });
    }
});

clientThread.Start();
NetworkTimeServer timeServer = new NetworkTimeServer();
_ = timeServer.Start(24420);

app.Run();

timeServer.Stop();



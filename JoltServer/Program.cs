// See https://aka.ms/new-console-template for more information

using JoltServer;

var app = new JoltApplication();
// var visualDebugger = new JoltRaylibDebugger(1200, 800, "Jolt Visual Debugger", app.targetFPS);
// app.AddSystem(visualDebugger); 

var unityDebugger = new JoltUnityDebugger(60,24419);
app.AddSystem(unityDebugger);
// Console.WriteLine("Hello, World!");
app.Run();
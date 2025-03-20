// See https://aka.ms/new-console-template for more information

using JoltServer;

var app = new JoltApplication();
var visualDebugger = new JoltVisualDebugger(1200, 800, "Jolt Visual Debugger", app.targetFPS);
app.AddSystem(visualDebugger); 
app.Run();
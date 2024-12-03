using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using UnityToolkit;

namespace ProjectZero.RPC
{
    public class GameRPC : ITaskSystem
    {
        private WebApplication _application;
        public Task task { get; private set; }


        public GameRPC(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSerilog(); // Add this line(Serilog)
            builder.Services.AddGrpc(); // Add this line(Grpc.AspNetCore)
            builder.Services.AddMagicOnion(); // Add this line(MagicOnion.Server)

            _application = builder.Build();

            // Configure the HTTP request pipeline.
            if (!_application.Environment.IsDevelopment())
            {
                _application.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _application.UseHsts();
            }

            _application.UseHttpsRedirection();

            _application.UseSerilogRequestLogging(); // Add this line(Serilog)

            _application.MapMagicOnionService(); // Add this line(MagicOnion.Server)
        }

        public Task Run()
        {
            task = _application.RunAsync();
            return task;
        }

        public void Dispose()
        {
        }
    }
}
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using UnityToolkit;

namespace FishGame
{
    public class GameRPC : ITaskSystem
    {
        private WebApplication _application;
        public Task task { get; private set; }


        public GameRPC(string[] args)
        {
            MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel(options =>
            {
                options.ConfigureEndpointDefaults(listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });
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
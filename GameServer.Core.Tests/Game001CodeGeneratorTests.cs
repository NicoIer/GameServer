using Game001.CodeGenerator;

namespace GameServer.Core.Tests;

[TestFixture]
public sealed class Game001CodeGeneratorTests
{
    [Test]
    public void GeneratesRequestRouterCommandRegistrationAndHandlers()
    {
        string repositoryRoot = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "code-generator-" + Guid.NewGuid().ToString("N"));
        try
        {
            string coreRuntime = Path.Combine(repositoryRoot, "Game001.Core", "UnityPackage", "Runtime");
            Directory.CreateDirectory(coreRuntime);
            Directory.CreateDirectory(Path.Combine(repositoryRoot, "Game001.Room", "Handlers"));
            File.WriteAllText(Path.Combine(repositoryRoot, "GameServer.slnx"), "<Solution />");
            File.WriteAllText(
                Path.Combine(coreRuntime, "Messages.cs"),
                """
                using GameServer.Core.Network;
                using GameServer.Core.Rooms;
                using MemoryPack;
                using Network;

                namespace Demo;

                [NetworkRequest(typeof(TeleportRsp))]
                [RoomRequestRoute(RoomRequestRouteKind.Room, RoomRequestRoomIdSource.BoundConnection)]
                public struct TeleportReq : INetworkReq { }
                public struct TeleportRsp : INetworkRsp { }

                [NetworkRequest(typeof(ListRsp))]
                [RoomRequestRoute(RoomRequestRouteKind.Worker)]
                public struct ListReq : INetworkReq { }
                public struct ListRsp : INetworkRsp { }

                [MemoryPackable]
                [RoomCommand]
                public partial struct UploadPositionCommand : IRoomCommand { }
                """);

            CodeGenerationContext context = CodeGenerationContext.Create(new[] { repositoryRoot });
            CSharpSourceCatalog sources = CSharpSourceCatalog.Load(context.CoreDirectory);
            var registrationStep = new RoomMessageRegistrationGenerationStep();
            var handlerStep = new RoomHandlerGenerationStep();

            CodeGenerationResult registrationResult = registrationStep.Execute(context, sources);
            CodeGenerationResult handlerResult = handlerStep.Execute(context, sources);

            Assert.That(registrationResult.Created, Is.EqualTo(1));
            Assert.That(handlerResult.Created, Is.EqualTo(3));
            string registration = File.ReadAllText(Path.Combine(
                context.RoomGeneratedDirectory,
                "Game001RoomMessageRegistration.g.cs"));
            Assert.That(registration, Does.Contain("router.Register<global::Demo.TeleportReq>"));
            Assert.That(registration, Does.Contain("router.RegisterWorker<global::Demo.ListReq, global::Demo.ListRsp>"));
            Assert.That(registration, Does.Contain("center.Register<global::Demo.UploadPositionCommand>"));

            string commandHandlerPath = Path.Combine(
                context.RoomHandlersDirectory,
                "Game001RoomCommandHandlers.UploadPosition.cs");
            File.AppendAllText(commandHandlerPath, "// custom handler marker\n");

            CodeGenerationResult repeatedRegistration = registrationStep.Execute(context, sources);
            CodeGenerationResult repeatedHandlers = handlerStep.Execute(context, sources);

            Assert.That(repeatedRegistration.Skipped, Is.EqualTo(1));
            Assert.That(repeatedHandlers.Skipped, Is.EqualTo(3));
            Assert.That(File.ReadAllText(commandHandlerPath), Does.Contain("custom handler marker"));
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, true);
            }
        }
    }
}

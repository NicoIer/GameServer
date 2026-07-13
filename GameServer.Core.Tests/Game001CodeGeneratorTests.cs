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
                using GameServer.Core.Ecs;
                using Friflo.Engine.ECS;
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


    [Test]
    public void GeneratesEntityRpcWireProxiesRegistrationsAndBothHandlerSides()
    {
        string repositoryRoot = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "rpc-code-generator-" + Guid.NewGuid().ToString("N"));
        string unityRoot = Path.Combine(repositoryRoot, "UnityClient");
        try
        {
            string coreRuntime = Path.Combine(repositoryRoot, "Game001.Core", "UnityPackage", "Runtime");
            Directory.CreateDirectory(coreRuntime);
            Directory.CreateDirectory(Path.Combine(repositoryRoot, "Game001.Room", "Handlers"));
            Directory.CreateDirectory(Path.Combine(unityRoot, "Assets", "Games", "Game001"));
            File.WriteAllText(Path.Combine(repositoryRoot, "GameServer.slnx"), "<Solution />");
            File.WriteAllText(
                Path.Combine(coreRuntime, "RpcContract.cs"),
                """
                using GameServer.Core.Rooms;
                using GameServer.Core.Ecs;
                using Friflo.Engine.ECS;

                namespace Demo;

                [EcsReplicatedComponent]
                public struct RpcComponent : IComponent { }

                [RoomRpcContract(typeof(global::Demo.RpcComponent))]
                public interface ICharacterRpc
                {
                    [ServerRpc]
                    void CmdMove(int sequence, float x);

                    [ClientRpc(IncludeOwner = false)]
                    void RpcHit(int amount);

                    [TargetRpc]
                    void TargetTeleport(float x, float y);
                }
                """);

            CodeGenerationContext context = CodeGenerationContext.Create(new[]
            {
                repositoryRoot,
                "--unity-root",
                unityRoot,
            });
            CSharpSourceCatalog sources = CSharpSourceCatalog.Load(context.CoreDirectory);
            var generationStep = new RoomRpcGenerationStep();
            var handlerStep = new RoomRpcHandlerGenerationStep();

            CodeGenerationResult generated = generationStep.Execute(context, sources);
            CodeGenerationResult handlers = handlerStep.Execute(context, sources);

            Assert.That(generated.Created, Is.EqualTo(3));
            Assert.That(handlers.Created, Is.EqualTo(3));
            string messages = File.ReadAllText(Path.Combine(
                context.GeneratedRuntimeDirectory,
                "Game001RpcMessages.g.cs"));
            string server = File.ReadAllText(Path.Combine(
                context.RoomGeneratedDirectory,
                "Game001RoomRpc.g.cs"));
            string client = File.ReadAllText(Path.Combine(
                context.UnityClientGeneratedDirectory,
                "Game001Rpc.g.cs"));
            Assert.That(messages, Does.Contain("struct Rpc_Character_CmdMove_Server"));
            Assert.That(client, Does.Contain("CharacterServerRpcProxy Character(int entityId)"));
            Assert.That(server, Does.Contain("CharacterClientRpcProxy Character(int entityId)"));
            Assert.That(server, Does.Contain("TargetRpcProxy Target(int connectionId)"));
            Assert.That(server, Does.Contain("SendObservers<global::Game001.Core.Generated.Rpc.Rpc_Character_RpcHit_Client"));
            Assert.That(server, Does.Contain("registry.Register<global::Game001.Core.Generated.Rpc.Rpc_Character_CmdMove_Server"));
            Assert.That(GetSyntaxErrors(messages), Is.Empty);
            Assert.That(GetSyntaxErrors(server), Is.Empty);
            Assert.That(GetSyntaxErrors(client), Is.Empty);
            Assert.That(File.Exists(Path.Combine(
                context.RoomHandlersDirectory,
                "Game001RoomServerRpcHandlers.HandleCharacterCmdMove.cs")), Is.True);
            Assert.That(File.Exists(Path.Combine(
                context.UnityClientRpcHandlersDirectory,
                "Game001ClientRpcHandlers.HandleCharacterRpcHit.cs")), Is.True);

            Assert.That(generationStep.Execute(context, sources).Skipped, Is.EqualTo(3));
            Assert.That(handlerStep.Execute(context, sources).Skipped, Is.EqualTo(3));
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, true);
            }
        }
    }

    [Test]
    public void RejectsRpcMethodOverloads()
    {
        string repositoryRoot = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "rpc-invalid-generator-" + Guid.NewGuid().ToString("N"));
        try
        {
            string coreRuntime = Path.Combine(repositoryRoot, "Game001.Core", "UnityPackage", "Runtime");
            Directory.CreateDirectory(coreRuntime);
            File.WriteAllText(Path.Combine(repositoryRoot, "GameServer.slnx"), "<Solution />");
            File.WriteAllText(
                Path.Combine(coreRuntime, "RpcContract.cs"),
                """
                using GameServer.Core.Rooms;
                using GameServer.Core.Ecs;
                using Friflo.Engine.ECS;
                namespace Demo;
                [EcsReplicatedComponent]
                public struct RpcComponent : IComponent { }
                [RoomRpcContract(typeof(global::Demo.RpcComponent))]
                public interface IInvalidRpc
                {
                    [ServerRpc] void CmdRun(int value);
                    [ServerRpc] void CmdRun(float value);
                }
                """);
            CSharpSourceCatalog sources = CSharpSourceCatalog.Load(
                Path.Combine(repositoryRoot, "Game001.Core"));

            Assert.That(
                () => RoomRpcCatalog.Collect(sources),
                Throws.TypeOf<InvalidOperationException>());
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, true);
            }
        }
    }

    [Test]
    public void RejectsRpcParameterWithoutMemoryPackContract()
    {
        string repositoryRoot = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "rpc-invalid-parameter-generator-" + Guid.NewGuid().ToString("N"));
        try
        {
            string coreRuntime = Path.Combine(repositoryRoot, "Game001.Core", "UnityPackage", "Runtime");
            Directory.CreateDirectory(coreRuntime);
            File.WriteAllText(Path.Combine(repositoryRoot, "GameServer.slnx"), "<Solution />");
            File.WriteAllText(
                Path.Combine(coreRuntime, "RpcContract.cs"),
                """
                using GameServer.Core.Rooms;
                using GameServer.Core.Ecs;
                using Friflo.Engine.ECS;
                namespace Demo;
                [EcsReplicatedComponent]
                public struct RpcComponent : IComponent { }
                public sealed class LocalPayload { }
                [RoomRpcContract(typeof(global::Demo.RpcComponent))]
                public interface IInvalidRpc
                {
                    [ClientRpc] void RpcPayload(global::Demo.LocalPayload payload);
                }
                """);
            CSharpSourceCatalog sources = CSharpSourceCatalog.Load(
                Path.Combine(repositoryRoot, "Game001.Core"));

            Assert.That(
                () => RoomRpcCatalog.Collect(sources),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("declare MemoryPackable"));
        }
        finally
        {
            if (Directory.Exists(repositoryRoot))
            {
                Directory.Delete(repositoryRoot, true);
            }
        }
    }

    private static IEnumerable<Microsoft.CodeAnalysis.Diagnostic> GetSyntaxErrors(string source)
    {
        return Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source)
            .GetDiagnostics()
            .Where(item => item.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
    }
}

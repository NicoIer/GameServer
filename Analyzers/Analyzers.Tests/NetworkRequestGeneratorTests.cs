using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Analyzers.Tests;

public class NetworkRequestGeneratorTests
{
    private const string SharedTypes = """
namespace GameServer.Core.Network
{
    [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
    public sealed class NetworkRequestAttribute : System.Attribute
    {
        public System.Type ResponseType { get; }

        public NetworkRequestAttribute(System.Type responseType)
        {
            ResponseType = responseType;
        }
    }
}

namespace Network
{
    public interface INetworkReq {}
    public interface INetworkRsp {}

    public enum ErrorCode
    {
        Success,
        InvalidArgument,
        InternalError,
        NotSupported,
        Timeout,
    }

    public sealed class ReqRspServerCenter
    {
        public delegate System.Threading.Tasks.ValueTask<(TRsp rsp, ErrorCode errorCode, string errorMsg)> ReqValueTaskHandleDelegate<TReq, TRsp>(
            int connectionId,
            TReq message);

        public void Register<TReq, TRsp>(ReqValueTaskHandleDelegate<TReq, TRsp> handleDelegate)
            where TReq : INetworkReq
            where TRsp : INetworkRsp
        {
        }
    }
}

namespace MemoryPack
{
    [System.AttributeUsage(System.AttributeTargets.Struct | System.AttributeTargets.Class)]
    public sealed class MemoryPackableAttribute : System.Attribute
    {
    }
}
""";

    [Fact]
    public void GeneratesInitializerForNetworkRequest()
    {
        string source = SharedTypes + Game001RoomMessagesSource();

        GeneratorDriverRunResult result = RunGenerator(source);
        string generated = Assert.Single(result.GeneratedTrees).GetText().ToString();

        Assert.Contains("namespace Game001.Core.Generated;", generated);
        Assert.Contains("public interface IGame001Handler", generated);
        Assert.Contains("global::System.Threading.Tasks.ValueTask<(global::Game001.Core.CreateRoomRsp rsp, global::Network.ErrorCode errorCode, string errorMsg)> HandleCreateRoom(int connectionId, global::Game001.Core.CreateRoomReq req);", generated);
        Assert.Contains("center.Register<global::Game001.Core.CreateRoomReq, global::Game001.Core.CreateRoomRsp>(handlers.HandleCreateRoom);", generated);
    }

    [Fact]
    public void DoesNotGenerateInitializerForUnmarkedGameServerCoreConnectionMessages()
    {
        string source = SharedTypes + GameServerCoreRoomConnectionMessagesSource();

        GeneratorDriverRunResult result = RunGenerator(source, "GameServer.Core");

        Assert.Empty(result.GeneratedTrees);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void ReportsInvalidResponseType()
    {
        string source = SharedTypes + TestDataSource("TestGame.Core", "InvalidResponseMessages.cs");

        GeneratorDriverRunResult result = RunGenerator(source, "TestGame.Core");

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC002");
    }

    [Fact]
    public void UnmarkedNetworkRequestIsIgnored()
    {
        string source = SharedTypes + TestDataSource("TestGame.Core", "MissingAttributeMessages.cs");

        GeneratorDriverRunResult result = RunGenerator(source, "TestGame.Core");

        Assert.Empty(result.GeneratedTrees);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void ReportsNetworkRequestAttributeOnNonRequestType()
    {
        string source = SharedTypes + TestDataSource("TestGame.Core", "InvalidRequestMessages.cs");

        GeneratorDriverRunResult result = RunGenerator(source, "TestGame.Core");

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC005");
    }

    [Fact]
    public void PartialRequestTypeRegistersOnce()
    {
        string source = SharedTypes + TestDataSource("TestGame.Core", "PartialRequestMessages.cs");

        GeneratorDriverRunResult result = RunGenerator(source, "TestGame.Core");
        string generated = Assert.Single(result.GeneratedTrees).GetText().ToString();

        Assert.DoesNotContain(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC003");
        Assert.Contains("public interface ITestGameHandler", generated);
        Assert.Equal(1, CountOccurrences(generated, "center.Register<global::TestGame.Core.RoomPingReq, global::TestGame.Core.RoomPingRsp>(handlers.HandleRoomPing);"));
    }

    [Fact]
    public void GeneratesGame002HandlerName()
    {
        string source = SharedTypes + Game002MessagesSource();

        GeneratorDriverRunResult result = RunGenerator(source, "Game002.Core");
        string generated = Assert.Single(result.GeneratedTrees).GetText().ToString();

        Assert.Contains("namespace Game002.Core.Generated;", generated);
        Assert.Contains("public interface IGame002Handler", generated);
        Assert.Contains("HandleMatchPing", generated);
    }

    [Fact]
    public void ReportsDuplicateResponseType()
    {
        string source = SharedTypes + DuplicateResponseMessagesSource();

        GeneratorDriverRunResult result = RunGenerator(source, "Game001.Core");

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC008");
    }

    [Fact]
    public void DoesNotGenerateRoomHandlerBridge()
    {
        string source = SharedTypes + Game001RoomMessagesSource() + """
namespace Game001.Room
{
    using Game001.Core;

    public sealed partial class Game001RoomReqRspHandlers
    {
        public System.Threading.Tasks.ValueTask<(CreateRoomRsp rsp, Network.ErrorCode errorCode, string errorMsg)> AnyCreateName(int connectionId, CreateRoomReq req) => throw null;
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source);

        Assert.DoesNotContain(result.GeneratedTrees, x => x.FilePath.Contains("Handlers/", StringComparison.Ordinal));
    }

    private static string Game001RoomMessagesSource()
    {
        string path = FindRepositoryFile("Game001.Core", "RoomMessages.cs");
        return File.ReadAllText(path);
    }

    private static string GameServerCoreRoomConnectionMessagesSource()
    {
        string path = FindRepositoryFile("GameServer.Core", "Network", "RoomConnectionMessages.cs");
        return File.ReadAllText(path);
    }

    private static string Game002MessagesSource()
    {
        return """
using GameServer.Core.Network;
using Network;

namespace Game002.Core;

[NetworkRequest(typeof(MatchPingRsp))]
public partial struct MatchPingReq : INetworkReq
{
}

public partial struct MatchPingRsp : INetworkRsp
{
}
""";
    }

    private static string DuplicateResponseMessagesSource()
    {
        return """
using GameServer.Core.Network;
using Network;

namespace Game001.Core;

[NetworkRequest(typeof(SharedRsp))]
public partial struct FirstReq : INetworkReq
{
}

[NetworkRequest(typeof(SharedRsp))]
public partial struct SecondReq : INetworkReq
{
}

public partial struct SharedRsp : INetworkRsp
{
}
""";
    }

    private static string TestDataSource(params string[] pathParts)
    {
        string path = FindRepositoryFile(new[] { "Analyzers", "Analyzers.Tests", "TestData" }.Concat(pathParts).ToArray());
        return File.ReadAllText(path);
    }

    private static string FindRepositoryFile(params string[] pathParts)
    {
        string? directory = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(directory))
        {
            string path = Path.Combine(new[] { directory }.Concat(pathParts).ToArray());
            if (File.Exists(path))
            {
                return path;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new FileNotFoundException("Could not find repository file.", Path.Combine(pathParts));
    }

    private static GeneratorDriverRunResult RunGenerator(string source, string assemblyName = "Game001.Core")
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new NetworkRequestGenerator());
        driver = driver.RunGenerators(compilation);
        return driver.GetRunResult();
    }

    private static IReadOnlyList<MetadataReference> GetReferences()
    {
        string? trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (!string.IsNullOrWhiteSpace(trustedPlatformAssemblies))
        {
            return trustedPlatformAssemblies
                .Split(Path.PathSeparator)
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToArray();
        }

        return new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        };
    }

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}

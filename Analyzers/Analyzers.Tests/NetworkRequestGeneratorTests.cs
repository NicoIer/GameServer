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
""";

    [Fact]
    public void GeneratesInitializerForNetworkRequest()
    {
        const string source = SharedTypes + """

using GameServer.Core.Network;
using Network;

namespace Game001.Core
{
    [NetworkRequest(typeof(CreateRoomRsp))]
    public partial struct CreateRoomReq : INetworkReq
    {
    }

    public partial struct CreateRoomRsp : INetworkRsp
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source);
        string generated = Assert.Single(result.GeneratedTrees).GetText().ToString();

        Assert.Contains("namespace Game001.Core.Generated;", generated);
        Assert.Contains("public interface INetworkReqRspHandlers", generated);
        Assert.Contains("global::System.Threading.Tasks.ValueTask<(global::Game001.Core.CreateRoomRsp rsp, global::Network.ErrorCode errorCode, string errorMsg)> Handle(int connectionId, global::Game001.Core.CreateRoomReq req);", generated);
        Assert.Contains("center.Register<global::Game001.Core.CreateRoomReq, global::Game001.Core.CreateRoomRsp>(handlers.Handle);", generated);
    }

    [Fact]
    public void ReportsInvalidResponseType()
    {
        const string source = SharedTypes + """

using GameServer.Core.Network;
using Network;

namespace Game001.Core
{
    [NetworkRequest(typeof(CreateRoomRsp))]
    public partial struct CreateRoomReq : INetworkReq
    {
    }

    public partial struct CreateRoomRsp
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source);

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC002");
    }

    [Fact]
    public void ReportsMissingNetworkRequestAttribute()
    {
        const string source = SharedTypes + """

using Network;

namespace Game001.Core
{
    public partial struct CreateRoomReq : INetworkReq
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source);

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC004");
    }

    [Fact]
    public void ReportsNetworkRequestAttributeOnNonRequestType()
    {
        const string source = SharedTypes + """

using GameServer.Core.Network;
using Network;

namespace Game001.Core
{
    [NetworkRequest(typeof(CreateRoomRsp))]
    public partial struct CreateRoomReq
    {
    }

    public partial struct CreateRoomRsp : INetworkRsp
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source);

        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC005");
    }

    [Fact]
    public void PartialRequestTypeRegistersOnce()
    {
        const string source = SharedTypes + """

using GameServer.Core.Network;
using Network;

namespace Game001.Core
{
    [NetworkRequest(typeof(RoomPingRsp))]
    public partial struct RoomPingReq : INetworkReq
    {
    }

    public partial struct RoomPingReq
    {
    }

    public partial struct RoomPingRsp : INetworkRsp
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source);
        string generated = Assert.Single(result.GeneratedTrees).GetText().ToString();

        Assert.DoesNotContain(result.Diagnostics, diagnostic => diagnostic.Id == "GSRPC003");
        Assert.Equal(1, CountOccurrences(generated, "center.Register<global::Game001.Core.RoomPingReq, global::Game001.Core.RoomPingRsp>(handlers.Handle);"));
    }

    [Fact]
    public void GeneratesHandlerBridgeForReqRspHandlers()
    {
        const string source = SharedTypes + """

using GameServer.Core.Network;
using Network;

namespace Game001.Core
{
    [NetworkRequest(typeof(CreateRoomRsp))]
    public partial struct CreateRoomReq : INetworkReq
    {
    }

    public partial struct CreateRoomRsp : INetworkRsp
    {
    }
}

namespace Game001.Room
{
    public sealed partial class Game001RoomReqRspHandlers
    {
    }
}
""";

        GeneratorDriverRunResult result = RunGenerator(source);
        string generated = result.GeneratedTrees
            .Select(x => x.GetText().ToString())
            .Single(x => x.Contains("Game001RoomReqRspHandlers"));

        Assert.Contains("public sealed partial class Game001RoomReqRspHandlers : global::Game001.Core.Generated.INetworkReqRspHandlers", generated);
        Assert.Contains("return CreateRoomReqRsp.Handle(this, connectionId, req);", generated);
        Assert.Contains("public static partial class CreateRoomReqRsp", generated);
    }

    private static GeneratorDriverRunResult RunGenerator(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview));
        CSharpCompilation compilation = CSharpCompilation.Create(
            "Game001.Core",
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Game001.CodeGenerator;

public sealed class RoomMessageCatalog
{
    public IReadOnlyList<RoomRequestInfo> Requests { get; }
    public IReadOnlyList<RoomCommandInfo> Commands { get; }

    private RoomMessageCatalog(List<RoomRequestInfo> requests, List<RoomCommandInfo> commands)
    {
        Requests = requests;
        Commands = commands;
    }

    public static RoomMessageCatalog Collect(CSharpSourceCatalog coreSources)
    {
        var requests = new SortedDictionary<string, RoomRequestInfo>(StringComparer.Ordinal);
        var commands = new SortedDictionary<string, RoomCommandInfo>(StringComparer.Ordinal);
        foreach (CSharpSourceFile sourceFile in coreSources.Files)
        {
            foreach (TypeDeclarationSyntax declaration in sourceFile.Root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                AttributeSyntax? requestAttribute = CSharpSyntax.FindAttribute(declaration, "NetworkRequest");
                if (requestAttribute != null)
                {
                    RoomRequestInfo request = CreateRequest(declaration, requestAttribute);
                    requests.Add(request.HandlerName, request);
                }

                if (CSharpSyntax.FindAttribute(declaration, "RoomCommand") != null)
                {
                    RoomCommandInfo command = CreateCommand(declaration);
                    commands.Add(command.HandlerName, command);
                }
            }
        }

        return new RoomMessageCatalog(requests.Values.ToList(), commands.Values.ToList());
    }

    private static RoomRequestInfo CreateRequest(
        TypeDeclarationSyntax declaration,
        AttributeSyntax requestAttribute)
    {
        if (requestAttribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax typeOf)
        {
            throw new InvalidOperationException(
                $"NetworkRequest must declare typeof(response): {declaration.Identifier.ValueText}");
        }

        AttributeSyntax? routeAttribute = CSharpSyntax.FindAttribute(declaration, "RoomRequestRoute");
        if (routeAttribute == null)
        {
            throw new InvalidOperationException(
                $"NetworkRequest must declare RoomRequestRoute: {declaration.Identifier.ValueText}");
        }

        SeparatedSyntaxList<AttributeArgumentSyntax> routeArguments =
            routeAttribute.ArgumentList?.Arguments ?? default;
        if (routeArguments.Count == 0)
        {
            throw new InvalidOperationException(
                $"RoomRequestRoute must declare route kind: {declaration.Identifier.ValueText}");
        }

        string kind = GetEnumValue(routeArguments[0].Expression);
        string roomIdSource = routeArguments.Count > 1
            ? GetEnumValue(routeArguments[1].Expression)
            : "None";
        if (kind == "Room" && roomIdSource == "None")
        {
            throw new InvalidOperationException(
                $"room request must declare RoomId source: {declaration.Identifier.ValueText}");
        }

        string requestName = declaration.Identifier.ValueText;
        string baseName = TrimSuffix(requestName, "Req");
        return new RoomRequestInfo(
            baseName,
            "Handle" + baseName,
            CSharpSyntax.GetTypeName(declaration),
            CSharpSyntax.GetReferencedTypeName(declaration, typeOf.Type),
            kind,
            roomIdSource,
            GetStringArgument(routeAttribute, "RoomIdMemberName", "RoomId"),
            GetStringArgument(routeAttribute, "DefaultRoomId", string.Empty),
            GetBoolArgument(routeAttribute, "CanCreateRoom", false),
            GetEnumArgument(routeAttribute, "SuccessConnectionAction", "None"),
            GetEnumArgument(routeAttribute, "RoomNotFoundErrorCode", "InvalidArgument"));
    }

    private static RoomCommandInfo CreateCommand(TypeDeclarationSyntax declaration)
    {
        if (CSharpSyntax.FindAttribute(declaration, "MemoryPackable") == null)
        {
            throw new InvalidOperationException(
                $"RoomCommand must declare MemoryPackable: {declaration.Identifier.ValueText}");
        }

        bool implementsCommand = declaration.BaseList?.Types.Any(
            item => GetSimpleName(item.Type.ToString()) == "IRoomCommand") == true;
        if (!implementsCommand)
        {
            throw new InvalidOperationException(
                $"RoomCommand must implement IRoomCommand: {declaration.Identifier.ValueText}");
        }

        string commandName = declaration.Identifier.ValueText;
        string baseName = TrimSuffix(commandName, "Command");
        return new RoomCommandInfo(
            baseName,
            "Handle" + baseName,
            CSharpSyntax.GetTypeName(declaration));
    }

    private static string GetStringArgument(AttributeSyntax attribute, string name, string defaultValue)
    {
        AttributeArgumentSyntax? argument = FindNamedArgument(attribute, name);
        if (argument == null)
        {
            return defaultValue;
        }

        if (argument?.Expression is LiteralExpressionSyntax literal && literal.Token.Value is string value)
        {
            return value;
        }

        if (argument.Expression is InvocationExpressionSyntax invocation &&
            invocation.Expression is IdentifierNameSyntax identifier &&
            identifier.Identifier.ValueText == "nameof" &&
            invocation.ArgumentList.Arguments.Count == 1)
        {
            return GetSimpleName(invocation.ArgumentList.Arguments[0].Expression.ToString());
        }

        throw new InvalidOperationException($"{name} must be a string literal or nameof expression");
    }

    private static bool GetBoolArgument(AttributeSyntax attribute, string name, bool defaultValue)
    {
        AttributeArgumentSyntax? argument = FindNamedArgument(attribute, name);
        if (argument == null)
        {
            return defaultValue;
        }

        if (argument?.Expression is LiteralExpressionSyntax literal && literal.Token.Value is bool value)
        {
            return value;
        }

        throw new InvalidOperationException($"{name} must be a bool literal");
    }

    private static string GetEnumArgument(AttributeSyntax attribute, string name, string defaultValue)
    {
        AttributeArgumentSyntax? argument = FindNamedArgument(attribute, name);
        return argument == null ? defaultValue : GetEnumValue(argument.Expression);
    }

    private static AttributeArgumentSyntax? FindNamedArgument(AttributeSyntax attribute, string name)
    {
        return attribute.ArgumentList?.Arguments.FirstOrDefault(
            item => item.NameEquals?.Name.Identifier.ValueText == name);
    }

    private static string GetEnumValue(ExpressionSyntax expression)
    {
        return GetSimpleName(expression.ToString());
    }

    private static string GetSimpleName(string name)
    {
        int separatorIndex = name.LastIndexOf('.');
        return separatorIndex < 0 ? name : name.Substring(separatorIndex + 1);
    }

    private static string TrimSuffix(string name, string suffix)
    {
        return name.EndsWith(suffix, StringComparison.Ordinal)
            ? name.Substring(0, name.Length - suffix.Length)
            : name;
    }
}

public sealed class RoomRequestInfo
{
    public string BaseName { get; }
    public string HandlerName { get; }
    public string RequestType { get; }
    public string ResponseType { get; }
    public string Kind { get; }
    public string RoomIdSource { get; }
    public string RoomIdMemberName { get; }
    public string DefaultRoomId { get; }
    public bool CanCreateRoom { get; }
    public string SuccessConnectionAction { get; }
    public string RoomNotFoundErrorCode { get; }

    public RoomRequestInfo(
        string baseName,
        string handlerName,
        string requestType,
        string responseType,
        string kind,
        string roomIdSource,
        string roomIdMemberName,
        string defaultRoomId,
        bool canCreateRoom,
        string successConnectionAction,
        string roomNotFoundErrorCode)
    {
        BaseName = baseName;
        HandlerName = handlerName;
        RequestType = requestType;
        ResponseType = responseType;
        Kind = kind;
        RoomIdSource = roomIdSource;
        RoomIdMemberName = roomIdMemberName;
        DefaultRoomId = defaultRoomId;
        CanCreateRoom = canCreateRoom;
        SuccessConnectionAction = successConnectionAction;
        RoomNotFoundErrorCode = roomNotFoundErrorCode;
    }
}

public sealed class RoomCommandInfo
{
    public string BaseName { get; }
    public string HandlerName { get; }
    public string CommandType { get; }

    public RoomCommandInfo(string baseName, string handlerName, string commandType)
    {
        BaseName = baseName;
        HandlerName = handlerName;
        CommandType = commandType;
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Game001.CodeGenerator;

public sealed class RoomRpcCatalog
{
    public IReadOnlyList<RoomRpcContractInfo> Contracts { get; }

    private RoomRpcCatalog(List<RoomRpcContractInfo> contracts)
    {
        Contracts = contracts;
    }

    public static RoomRpcCatalog Collect(CSharpSourceCatalog coreSources)
    {
        var contracts = new List<RoomRpcContractInfo>();
        var componentTypes = new HashSet<string>(StringComparer.Ordinal);
        var serializableTypes = new HashSet<string>(StringComparer.Ordinal);
        var knownTypes = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var accessorNames = new HashSet<string>(StringComparer.Ordinal);
        var serverHashes = new Dictionary<ushort, string>();
        var clientHashes = new Dictionary<ushort, string>();

        foreach (CSharpSourceFile sourceFile in coreSources.Files)
        {
            foreach (TypeDeclarationSyntax declaration in sourceFile.Root.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                string typeName = CSharpSyntax.GetTypeName(declaration);
                if (!knownTypes.TryGetValue(declaration.Identifier.ValueText, out List<string>? matches))
                {
                    matches = new List<string>();
                    knownTypes.Add(declaration.Identifier.ValueText, matches);
                }
                matches.Add(typeName);

                if (CSharpSyntax.FindAttribute(declaration, "MemoryPackable") != null)
                {
                    serializableTypes.Add(typeName);
                }

                if (declaration.BaseList?.Types.Any(
                        item => GetSimpleName(item.Type.ToString()) == "IComponent") == true &&
                    CSharpSyntax.FindAttribute(declaration, "EcsReplicatedComponent") != null)
                {
                    componentTypes.Add(typeName);
                }

                if (declaration.BaseList?.Types.Any(
                        item => GetSimpleName(item.Type.ToString()) == "IRoomCommand") == true)
                {
                    AddExistingHash(serverHashes, typeName);
                }

                if (declaration.BaseList?.Types.Any(
                        item => GetSimpleName(item.Type.ToString()) == "IRoomPush") == true)
                {
                    AddExistingHash(clientHashes, typeName);
                }
            }

            foreach (EnumDeclarationSyntax declaration in sourceFile.Root.DescendantNodes().OfType<EnumDeclarationSyntax>())
            {
                string typeName = GetTypeName(declaration);
                if (!knownTypes.TryGetValue(declaration.Identifier.ValueText, out List<string>? matches))
                {
                    matches = new List<string>();
                    knownTypes.Add(declaration.Identifier.ValueText, matches);
                }

                matches.Add(typeName);
                serializableTypes.Add(typeName);
            }
        }

        foreach (CSharpSourceFile sourceFile in coreSources.Files)
        {
            foreach (InterfaceDeclarationSyntax declaration in sourceFile.Root
                         .DescendantNodes()
                         .OfType<InterfaceDeclarationSyntax>())
            {
                AttributeSyntax? contractAttribute = CSharpSyntax.FindAttribute(declaration, "RoomRpcContract");
                if (contractAttribute == null)
                {
                    continue;
                }

                RoomRpcContractInfo contract = CreateContract(
                    declaration,
                    contractAttribute,
                    componentTypes,
                    knownTypes,
                    serializableTypes);
                if (contract.AccessorName.Length == 0 || !accessorNames.Add(contract.AccessorName))
                {
                    throw new InvalidOperationException(
                        $"RPC contract accessor name must be unique and non-empty: {contract.AccessorName}");
                }
                foreach (RoomRpcMethodInfo method in contract.Methods)
                {
                    Dictionary<ushort, string> hashes = method.Direction == RoomRpcDirection.Server
                        ? serverHashes
                        : clientHashes;
                    ushort hash = GetStableHashCode16(method.MessageTypeName);
                    if (hashes.TryGetValue(hash, out string? existing) && existing != method.MessageTypeName)
                    {
                        throw new InvalidOperationException(
                            $"RPC stable id collision hash={hash} first={existing} second={method.MessageTypeName}");
                    }

                    hashes[hash] = method.MessageTypeName;
                }

                contracts.Add(contract);
            }
        }

        contracts.Sort((left, right) => string.CompareOrdinal(left.ContractType, right.ContractType));
        return new RoomRpcCatalog(contracts);
    }

    private static RoomRpcContractInfo CreateContract(
        InterfaceDeclarationSyntax declaration,
        AttributeSyntax contractAttribute,
        HashSet<string> componentTypes,
        Dictionary<string, List<string>> knownTypes,
        HashSet<string> serializableTypes)
    {
        if (contractAttribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax typeOf)
        {
            throw new InvalidOperationException(
                $"RoomRpcContract must declare typeof(required component): {declaration.Identifier.ValueText}");
        }

        string contractType = CSharpSyntax.GetTypeName(declaration);
        string componentType = FormatType(typeOf.Type, knownTypes);
        string normalizedComponentType = componentType.StartsWith("global::", StringComparison.Ordinal)
            ? componentType.Substring("global::".Length)
            : componentType;
        if (!componentTypes.Contains(normalizedComponentType))
        {
            throw new InvalidOperationException(
                $"RoomRpcContract component must be a replicated IComponent in Game001.Core: {normalizedComponentType}");
        }
        string accessorName = GetAccessorName(declaration.Identifier.ValueText);
        var methods = new List<RoomRpcMethodInfo>();
        var methodNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (MethodDeclarationSyntax method in declaration.Members.OfType<MethodDeclarationSyntax>())
        {
            methods.Add(CreateMethod(accessorName, method, methodNames, knownTypes, serializableTypes));
        }

        if (methods.Any(item => item.Direction == RoomRpcDirection.Target) &&
            methods.Any(item => item.Direction == RoomRpcDirection.Client && item.MethodName == "Target"))
        {
            throw new InvalidOperationException(
                $"ClientRpc method name Target conflicts with the TargetRpc selector: {contractType}.Target");
        }

        methods.Sort((left, right) => string.CompareOrdinal(left.MethodName, right.MethodName));
        return new RoomRpcContractInfo(contractType, componentType, accessorName, methods);
    }

    private static RoomRpcMethodInfo CreateMethod(
        string accessorName,
        MethodDeclarationSyntax method,
        HashSet<string> methodNames,
        Dictionary<string, List<string>> knownTypes,
        HashSet<string> serializableTypes)
    {
        if (!methodNames.Add(method.Identifier.ValueText))
        {
            throw new InvalidOperationException(
                $"RPC method overloads are not supported: {accessorName}.{method.Identifier.ValueText}");
        }

        if (!method.ReturnType.IsKind(SyntaxKind.PredefinedType) ||
            ((PredefinedTypeSyntax)method.ReturnType).Keyword.Kind() != SyntaxKind.VoidKeyword ||
            method.TypeParameterList != null ||
            method.Modifiers.Any(token => token.IsKind(SyntaxKind.StaticKeyword)) ||
            method.Body != null ||
            method.ExpressionBody != null)
        {
            throw new InvalidOperationException(
                $"RPC method must be non-static, non-generic and return void: {method.Identifier.ValueText}");
        }

        AttributeSyntax? serverRpc = FindMethodAttribute(method, "ServerRpc");
        AttributeSyntax? clientRpc = FindMethodAttribute(method, "ClientRpc");
        AttributeSyntax? targetRpc = FindMethodAttribute(method, "TargetRpc");
        int attributeCount = (serverRpc == null ? 0 : 1) +
                             (clientRpc == null ? 0 : 1) +
                             (targetRpc == null ? 0 : 1);
        if (attributeCount != 1)
        {
            throw new InvalidOperationException(
                $"RPC method must declare exactly one direction attribute: {method.Identifier.ValueText}");
        }

        var parameters = new List<RoomRpcParameterInfo>();
        var fieldNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "EntityId",
        };
        foreach (ParameterSyntax parameter in method.ParameterList.Parameters)
        {
            string parameterName = parameter.Identifier.ValueText;
            if (parameter.Type == null ||
                parameter.Modifiers.Any(token => token.IsKind(SyntaxKind.RefKeyword)) ||
                parameter.Modifiers.Any(token => token.IsKind(SyntaxKind.OutKeyword)) ||
                parameter.Modifiers.Any(token => token.IsKind(SyntaxKind.InKeyword)) ||
                parameter.Modifiers.Any(token => token.IsKind(SyntaxKind.ParamsKeyword)) ||
                parameter.Default != null ||
                IsReservedParameterName(parameterName))
            {
                throw new InvalidOperationException(
                    $"invalid RPC parameter: {method.Identifier.ValueText}.{parameterName}");
            }

            ValidateSerializableType(parameter.Type, knownTypes, serializableTypes);
            string fieldName = UpperFirst(parameterName);
            if (!fieldNames.Add(fieldName))
            {
                throw new InvalidOperationException(
                    $"RPC parameters produce duplicate wire field: {method.Identifier.ValueText}.{fieldName}");
            }

            parameters.Add(new RoomRpcParameterInfo(
                parameterName,
                fieldName,
                FormatType(parameter.Type, knownTypes)));
        }

        RoomRpcDirection direction;
        bool option;
        if (serverRpc != null)
        {
            direction = RoomRpcDirection.Server;
            option = GetBoolArgument(serverRpc, "RequiresAuthority", true);
        }
        else if (clientRpc != null)
        {
            direction = RoomRpcDirection.Client;
            option = GetBoolArgument(clientRpc, "IncludeOwner", true);
        }
        else
        {
            direction = RoomRpcDirection.Target;
            option = true;
        }

        string messageShortName = "Rpc_" + accessorName + "_" + method.Identifier.ValueText + "_" + direction;
        return new RoomRpcMethodInfo(
            method.Identifier.ValueText,
            "Handle" + accessorName + method.Identifier.ValueText,
            direction,
            option,
            parameters,
            "Game001.Core.Generated.Rpc." + messageShortName,
            messageShortName);
    }

    private static AttributeSyntax? FindMethodAttribute(MethodDeclarationSyntax method, string name)
    {
        foreach (AttributeSyntax attribute in method.AttributeLists.SelectMany(list => list.Attributes))
        {
            string attributeName = attribute.Name.ToString();
            int separatorIndex = attributeName.LastIndexOf('.');
            if (separatorIndex >= 0)
            {
                attributeName = attributeName.Substring(separatorIndex + 1);
            }

            if (attributeName == name || attributeName == name + "Attribute")
            {
                return attribute;
            }
        }

        return null;
    }

    private static bool GetBoolArgument(AttributeSyntax attribute, string name, bool defaultValue)
    {
        AttributeArgumentSyntax? argument = attribute.ArgumentList?.Arguments.FirstOrDefault(
            item => item.NameEquals?.Name.Identifier.ValueText == name);
        if (argument == null)
        {
            return defaultValue;
        }

        if (argument.Expression is LiteralExpressionSyntax literal && literal.Token.Value is bool value)
        {
            return value;
        }

        throw new InvalidOperationException($"{name} must be a bool literal");
    }

    private static string FormatType(
        TypeSyntax type,
        Dictionary<string, List<string>> knownTypes)
    {
        return type switch
        {
            PredefinedTypeSyntax predefined => predefined.Keyword.ValueText,
            IdentifierNameSyntax identifier => ResolveIdentifier(identifier.Identifier.ValueText, knownTypes),
            QualifiedNameSyntax qualified => "global::" + qualified,
            AliasQualifiedNameSyntax aliasQualified => aliasQualified.ToString(),
            NullableTypeSyntax nullable => FormatType(nullable.ElementType, knownTypes) + "?",
            ArrayTypeSyntax array => FormatType(array.ElementType, knownTypes) + array.RankSpecifiers.ToFullString(),
            _ => throw new InvalidOperationException($"unsupported RPC parameter type syntax: {type}"),
        };
    }

    private static string ResolveIdentifier(
        string identifier,
        Dictionary<string, List<string>> knownTypes)
    {
        if (!knownTypes.TryGetValue(identifier, out List<string>? matches) || matches.Count != 1)
        {
            throw new InvalidOperationException(
                $"RPC type must resolve to exactly one Game001.Core type or use a fully qualified name: {identifier}");
        }

        return "global::" + matches[0];
    }

    private static void ValidateSerializableType(
        TypeSyntax type,
        Dictionary<string, List<string>> knownTypes,
        HashSet<string> serializableTypes)
    {
        if (type is PredefinedTypeSyntax)
        {
            return;
        }

        if (type is NullableTypeSyntax nullable)
        {
            ValidateSerializableType(nullable.ElementType, knownTypes, serializableTypes);
            return;
        }

        if (type is ArrayTypeSyntax array)
        {
            ValidateSerializableType(array.ElementType, knownTypes, serializableTypes);
            return;
        }

        string typeName;
        if (type is IdentifierNameSyntax identifier)
        {
            typeName = ResolveIdentifier(identifier.Identifier.ValueText, knownTypes)
                .Substring("global::".Length);
        }
        else if (type is QualifiedNameSyntax or AliasQualifiedNameSyntax)
        {
            typeName = type.ToString();
            if (typeName.StartsWith("global::", StringComparison.Ordinal))
            {
                typeName = typeName.Substring("global::".Length);
            }
        }
        else
        {
            throw new InvalidOperationException($"unsupported RPC parameter type syntax: {type}");
        }

        if (!serializableTypes.Contains(typeName))
        {
            throw new InvalidOperationException(
                $"RPC parameter type must be a Game001.Core enum or declare MemoryPackable: {typeName}");
        }
    }

    private static string GetTypeName(EnumDeclarationSyntax declaration)
    {
        var parts = new List<string>();
        foreach (BaseNamespaceDeclarationSyntax item in declaration.Ancestors()
                     .OfType<BaseNamespaceDeclarationSyntax>()
                     .Reverse())
        {
            parts.Add(item.Name.ToString());
        }

        foreach (TypeDeclarationSyntax item in declaration.Ancestors()
                     .OfType<TypeDeclarationSyntax>()
                     .Reverse())
        {
            parts.Add(item.Identifier.ValueText);
        }

        parts.Add(declaration.Identifier.ValueText);
        return string.Join(".", parts);
    }

    private static string GetAccessorName(string contractName)
    {
        string name = contractName.Length > 1 && contractName[0] == 'I' && char.IsUpper(contractName[1])
            ? contractName.Substring(1)
            : contractName;
        if (name.EndsWith("Contract", StringComparison.Ordinal))
        {
            name = name.Substring(0, name.Length - "Contract".Length);
        }

        if (name.EndsWith("Rpc", StringComparison.Ordinal))
        {
            name = name.Substring(0, name.Length - "Rpc".Length);
        }

        return name;
    }

    private static bool IsReservedParameterName(string name)
    {
        return string.Equals(name, "entityId", StringComparison.OrdinalIgnoreCase) ||
               name == "message" ||
               name == "context" ||
               name == "_sender" ||
               name == "_entityId" ||
               name == "_connectionId";
    }

    private static string GetSimpleName(string name)
    {
        int separatorIndex = name.LastIndexOf('.');
        return separatorIndex < 0 ? name : name.Substring(separatorIndex + 1);
    }

    private static string UpperFirst(string value)
    {
        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    private static ushort GetStableHashCode16(string text)
    {
        unchecked
        {
            uint hash = 0x811c9dc5;
            const uint prime = 0x1000193;
            for (int i = 0; i < text.Length; i++)
            {
                hash ^= (byte)text[i];
                hash *= prime;
            }

            return (ushort)(((int)hash >> 16) ^ (int)hash);
        }
    }

    private static void AddExistingHash(Dictionary<ushort, string> hashes, string typeName)
    {
        ushort hash = GetStableHashCode16(typeName);
        if (hashes.TryGetValue(hash, out string? existing) && existing != typeName)
        {
            throw new InvalidOperationException(
                $"room message stable id collision hash={hash} first={existing} second={typeName}");
        }

        hashes[hash] = typeName;
    }
}

public enum RoomRpcDirection
{
    Server,
    Client,
    Target,
}

public sealed record RoomRpcContractInfo(
    string ContractType,
    string ComponentType,
    string AccessorName,
    IReadOnlyList<RoomRpcMethodInfo> Methods);

public sealed record RoomRpcMethodInfo(
    string MethodName,
    string HandlerName,
    RoomRpcDirection Direction,
    bool Option,
    IReadOnlyList<RoomRpcParameterInfo> Parameters,
    string MessageTypeName,
    string MessageShortName);

public sealed record RoomRpcParameterInfo(string Name, string FieldName, string TypeName);

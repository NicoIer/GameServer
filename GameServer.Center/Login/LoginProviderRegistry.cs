using GameServer.Core.Protocol;

namespace GameServer.Center.Login;

public sealed class LoginProviderRegistry
{
    private readonly Dictionary<string, ILoginProvider> _providers = new(StringComparer.OrdinalIgnoreCase);

    public void Register(ILoginProvider provider)
    {
        _providers[provider.LoginType] = provider;
    }

    public LoginResult Login(AuthRequest request)
    {
        if (request.LoginType.Length == 0)
        {
            return new LoginResult(ErrorCode.InvalidRequest, 0);
        }

        if (!_providers.TryGetValue(request.LoginType, out ILoginProvider? provider))
        {
            return new LoginResult(ErrorCode.Unauthorized, 0);
        }

        return provider.Login(request);
    }
}

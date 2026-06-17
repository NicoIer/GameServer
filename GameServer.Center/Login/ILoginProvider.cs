using GameServer.Core.Protocol;

namespace GameServer.Center.Login;

public interface ILoginProvider
{
    string LoginType { get; }
    LoginResult Login(AuthRequest request);
}

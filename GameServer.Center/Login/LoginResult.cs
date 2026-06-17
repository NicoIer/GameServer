namespace GameServer.Center.Login;

public readonly struct LoginResult
{
    public LoginResult(int error, long uid)
    {
        Error = error;
        Uid = uid;
    }

    public int Error { get; }
    public long Uid { get; }
}

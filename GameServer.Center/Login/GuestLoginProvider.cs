using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using GameServer.Core.Protocol;

namespace GameServer.Center.Login;

public sealed class GuestLoginProvider : ILoginProvider
{
    public string LoginType
    {
        get { return "guest"; }
    }

    public LoginResult Login(AuthRequest request)
    {
        string guestKey = request.Credential.Length > 0 ? request.Credential : request.DeviceId;
        if (guestKey.Length == 0)
        {
            return new LoginResult(ErrorCode.InvalidRequest, 0);
        }

        long uid = CreateGuestUid(guestKey);
        return new LoginResult(ErrorCode.Success, uid);
    }

    private static long CreateGuestUid(string guestKey)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(guestKey));
        ulong raw = BinaryPrimitives.ReadUInt64LittleEndian(bytes);
        long uid = (long)(raw & 0x7FFF_FFFF_FFFF_FFFFUL);

        if (uid == 0)
        {
            uid = 1;
        }

        return uid;
    }
}

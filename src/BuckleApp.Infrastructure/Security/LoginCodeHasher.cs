using System.Security.Cryptography;
using System.Text;
using BuckleApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BuckleApp.Infrastructure.Security;

public class LoginCodeHasher : ILoginCodeHasher
{
    private readonly byte[] _key;

    public LoginCodeHasher(IConfiguration configuration)
    {
        _key = Convert.FromBase64String(configuration["Authentication:LoginCodeHashKey"]
            ?? throw new InvalidOperationException("Configure Authentication:LoginCodeHashKey."));
        if (_key.Length < 32)
            throw new InvalidOperationException("Login code hash key must contain at least 32 random bytes.");
    }

    public string Hash(Guid userId, Guid codeId, string code) => Convert.ToHexString(
        HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes($"{userId:N}:{codeId:N}:{code}")));

    public bool Verify(Guid userId, Guid codeId, string code, string hash) =>
        CryptographicOperations.FixedTimeEquals(Convert.FromHexString(Hash(userId, codeId, code)),
            Convert.FromHexString(hash));
}

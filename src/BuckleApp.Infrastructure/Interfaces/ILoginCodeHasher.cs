namespace BuckleApp.Infrastructure.Interfaces;

public interface ILoginCodeHasher
{
    string Hash(Guid userId, Guid codeId, string code);
    bool Verify(Guid userId, Guid codeId, string code, string hash);
}

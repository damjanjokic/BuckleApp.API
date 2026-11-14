using BuckleApp.Core.IRepositories;

namespace BuckleApp.Application.Features.Authentication.LogoutUser;

public class LogoutUserService : ILogoutUserService
{
    private readonly IAuthRepository _authRepository;

    public LogoutUserService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task ExecuteAsync(LogoutUserRequest request)
    {
        var token = await _authRepository.FindRefreshTokenAsync(AuthenticationRules.HashRefreshToken(request.RefreshToken));
        if (token == null) return;
        await _authRepository.WithUserLockAsync(token.UserId, async () =>
        {
            foreach (var member in await _authRepository.GetRefreshFamilyAsync(token.UserId, token.FamilyId))
                member.RevokedAt ??= DateTime.UtcNow;
            return true;
        });
    }
}

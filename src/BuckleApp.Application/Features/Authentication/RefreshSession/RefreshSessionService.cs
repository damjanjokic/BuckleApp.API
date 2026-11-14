using BuckleApp.Application.Dtos;
using BuckleApp.Core.IRepositories;
using BuckleApp.Application.Features.Authentication.Services;

namespace BuckleApp.Application.Features.Authentication.RefreshSession;

public class RefreshSessionService : IRefreshSessionService
{
    private readonly IAuthRepository _authRepository;
    private readonly IAuthenticationSessionService _authenticationSessionService;

    public RefreshSessionService(IAuthRepository authRepository, IAuthenticationSessionService authenticationSessionService)
    {
        _authRepository = authRepository;
        _authenticationSessionService = authenticationSessionService;
    }

    public async Task<AuthenticationResponse> ExecuteAsync(RefreshSessionRequest request)
    {
        var hash = AuthenticationRules.HashRefreshToken(request.RefreshToken);
        var found = await _authRepository.FindRefreshTokenAsync(hash);
        if (found == null) throw AuthenticationRules.InvalidCredentials();
        var response = await _authRepository.WithUserLockAsync<AuthenticationResponse?>(found.UserId, async () =>
        {
            var token = (await _authRepository.FindRefreshTokenAsync(hash))!;
            var user = await _authRepository.FindByIdAsync(token.UserId);
            var now = DateTime.UtcNow;
            if (token.RevokedAt != null || token.ExpiresAt <= now || !await AuthenticationRules.IsEligibleSessionUserAsync(user, _authRepository) ||
                token.SecurityStamp != user!.SecurityStamp)
            {
                foreach (var member in await _authRepository.GetRefreshFamilyAsync(token.UserId, token.FamilyId))
                    member.RevokedAt ??= now;
                return null;
            }
            token.RevokedAt = now;
            return await _authenticationSessionService.CreateAsync(
                user!, token.FamilyId, token.ExpiresAt);
        });
        return response ?? throw AuthenticationRules.InvalidCredentials();
    }
}

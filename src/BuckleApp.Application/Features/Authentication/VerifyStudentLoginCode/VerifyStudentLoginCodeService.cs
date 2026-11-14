using BuckleApp.Application.Dtos;
using BuckleApp.Core.IRepositories;
using BuckleApp.Infrastructure.Interfaces;
using BuckleApp.Application.Features.Authentication.Services;

namespace BuckleApp.Application.Features.Authentication.VerifyStudentLoginCode;

public class VerifyStudentLoginCodeService : IVerifyStudentLoginCodeService
{
    private readonly IAuthRepository _authRepository;
    private readonly ILoginCodeHasher _loginCodeHasher;
    private readonly IAuthenticationSessionService _authenticationSessionService;

    public VerifyStudentLoginCodeService(IAuthRepository authRepository, ILoginCodeHasher loginCodeHasher, IAuthenticationSessionService authenticationSessionService)
    {
        _authRepository = authRepository;
        _loginCodeHasher = loginCodeHasher;
        _authenticationSessionService = authenticationSessionService;
    }

    public async Task<AuthenticationResponse> ExecuteAsync(VerifyStudentLoginCodeRequest request)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email.Trim());
        if (user == null) throw AuthenticationRules.InvalidCredentials();
        var response = await _authRepository.WithUserLockAsync<AuthenticationResponse?>(user.Id, async () =>
        {
            var current = await _authRepository.FindByIdAsync(user.Id);
            if (!await AuthenticationRules.IsEligibleStudentAsync(current, _authRepository)) return null;
            var now = DateTime.UtcNow;
            var entry = (await _authRepository.GetLoginCodesAsync(user.Id)).SingleOrDefault(x => x.ConsumedAt == null);
            if (entry == null || entry.ExpiresAt <= now || entry.FailedAttempts >= 5) return null;
            if (!_loginCodeHasher.Verify(user.Id, entry.Id, request.Code, entry.CodeHash))
            {
                entry.FailedAttempts++;
                return null;
            }
            entry.ConsumedAt = now;
            return await _authenticationSessionService.CreateAsync(
                current!, Guid.NewGuid());
        });
        return response ?? throw AuthenticationRules.InvalidCredentials();
    }
}

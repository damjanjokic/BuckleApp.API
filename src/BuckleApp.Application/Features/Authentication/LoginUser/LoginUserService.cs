using BuckleApp.Application.Dtos;
using BuckleApp.Application.Features.Authentication.Services;
using BuckleApp.Application.Features.Authentication.VerifyStudentLoginCode;
using BuckleApp.Core.IRepositories;

namespace BuckleApp.Application.Features.Authentication.LoginUser;

public class LoginUserService : ILoginUserService
{
    private readonly IAuthRepository _authRepository;
    private readonly IVerifyStudentLoginCodeService _verifyStudentLoginCodeService;
    private readonly IAuthenticationSessionService _authenticationSessionService;

    public LoginUserService(IAuthRepository authRepository,
        IVerifyStudentLoginCodeService verifyStudentLoginCodeService,
        IAuthenticationSessionService authenticationSessionService)
    {
        _authRepository = authRepository;
        _verifyStudentLoginCodeService = verifyStudentLoginCodeService;
        _authenticationSessionService = authenticationSessionService;
    }

    public async Task<AuthenticationResponse> ExecuteAsync(LoginUserRequest request)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email.Trim());
        if (user == null) throw AuthenticationRules.InvalidCredentials();
        var roles = (await _authRepository.GetUserRoles(user)).ToList();
        if (roles.Contains("Student"))
            return await _verifyStudentLoginCodeService.ExecuteAsync(new VerifyStudentLoginCodeRequest
            {
                Email = user.Email!, Code = request.Password
            });

        var response = await _authRepository.WithUserLockAsync<AuthenticationResponse?>(user.Id, async () =>
        {
            var current = await _authRepository.FindByIdAsync(user.Id);

            if (!await AuthenticationRules.IsEligibleSessionUserAsync(current, _authRepository) ||
                (await _authRepository.GetUserRoles(current!)).Contains("Student")) return null;
            if (!await _authRepository.CheckPasswordSignInAsync(current!, request.Password, true)) return null;
            return await _authenticationSessionService.CreateAsync(
                current!, Guid.NewGuid());
        });

        return response ?? throw AuthenticationRules.InvalidCredentials();
    }
}

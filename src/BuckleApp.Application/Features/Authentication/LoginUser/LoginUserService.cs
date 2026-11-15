using System.Net;
using Microsoft.Extensions.Logging;
using BuckleApp.Application.Exceptions;
using BuckleApp.Application.Dtos;
using BuckleApp.Application.Features.Authentication.Services;
using BuckleApp.Application.Features.Authentication.VerifyStudentLoginCode;
using BuckleApp.Core.IRepositories;

namespace BuckleApp.Application.Features.Authentication.LoginUser;

public class LoginUserService : ILoginUserService
{
    private readonly ILogger<LoginUserService> _logger;
    private readonly IAuthRepository _authRepository;
    private readonly IVerifyStudentLoginCodeService _verifyStudentLoginCodeService;
    private readonly IAuthenticationSessionService _authenticationSessionService;

    public LoginUserService(IAuthRepository authRepository,
        IVerifyStudentLoginCodeService verifyStudentLoginCodeService,
        IAuthenticationSessionService authenticationSessionService, ILogger<LoginUserService> logger)
    {
        _logger = logger;
        _authRepository = authRepository;
        _verifyStudentLoginCodeService = verifyStudentLoginCodeService;
        _authenticationSessionService = authenticationSessionService;
    }

    public async Task<AuthenticationResponse> ExecuteAsync(LoginUserRequest request)
    {
        _logger.LogDebug("Login attempt started");
        try
        {
            var user = await _authRepository.FindByEmailAsync(request.Email.Trim());
            if (user == null) throw AuthenticationRules.InvalidCredentials();
            var roles = (await _authRepository.GetUserRoles(user)).ToList();
            if (roles.Contains("Student"))
            {
                _logger.LogDebug("Verifying student login code for user {UserId}", user.Id);
                var studentResponse = await _verifyStudentLoginCodeService.ExecuteAsync(new VerifyStudentLoginCodeRequest
                {
                    Email = user.Email!, Code = request.Password
                });
                _logger.LogInformation("User {UserId} logged in using {AuthenticationMethod}", user.Id, "StudentCode");
                return studentResponse;
            }

            _logger.LogDebug("Verifying password login for user {UserId}", user.Id);

            var response = await _authRepository.WithUserLockAsync<AuthenticationResponse?>(user.Id, async () =>
            {
                var current = await _authRepository.FindByIdAsync(user.Id);

                if (!await AuthenticationRules.IsEligibleSessionUserAsync(current, _authRepository) ||
                    (await _authRepository.GetUserRoles(current!)).Contains("Student")) return null;
                if (!await _authRepository.CheckPasswordSignInAsync(current!, request.Password, true)) return null;
                return await _authenticationSessionService.CreateAsync(
                    current!, Guid.NewGuid());
            });

            if (response == null) throw AuthenticationRules.InvalidCredentials();
            _logger.LogInformation("User {UserId} logged in using {AuthenticationMethod}", user.Id, "Password");
            return response;
        }
        catch (RestException ex) when (ex.Code == HttpStatusCode.Unauthorized)
        {
            // Expected rejection only; the middleware owns exception/error logging.
            // Keep the reason generic and never record the request or credentials.
            _logger.LogDebug("Login rejected: invalid credentials or ineligible account");
            throw;
        }
    }
}

using BuckleApp.Application.Dtos;
using BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;
using BuckleApp.Application.Features.Authentication.RefreshSession;
using BuckleApp.Application.Features.Authentication.LogoutUser;
using BuckleApp.API.Requests.Auth;
using BuckleApp.Application.Features.Authentication.LoginUser;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Application.Features.Authentication.ResetPassword;

namespace BuckleApp.Application.Features.Authentication;

public class AuthService : IAuthService
{
    private readonly IRequestStudentLoginCodeService _requestStudentLoginCode;
    private readonly IRefreshSessionService _refreshSession;
    private readonly ILogoutUserService _logoutUser;
    private readonly ILoginUserService _loginUserService;
    private readonly IRegisterUserService _registerUserService;
    private readonly IResetPasswordService _resetPasswordService;

    public AuthService(ILoginUserService loginUserService, IRegisterUserService registerUserService, IResetPasswordService resetPasswordService,
        IRequestStudentLoginCodeService requestStudentLoginCode,
        IRefreshSessionService refreshSession,
        ILogoutUserService logoutUser)
    {
        _loginUserService = loginUserService;
        _registerUserService = registerUserService;
        _resetPasswordService = resetPasswordService;
        _requestStudentLoginCode = requestStudentLoginCode;
        _refreshSession = refreshSession;
        _logoutUser = logoutUser;
    }

    public async Task<AuthenticationResponse> LoginUser(LoginUserRequest request)
        => await _loginUserService.ExecuteAsync(request);

    public async Task<Guid> RegisterUser(RegisterUserRequest request)
        => await _registerUserService.ExecuteAsync(request);

    public async Task<bool> ResetPassword(ResetPasswordRequest request)
        => await _resetPasswordService.ExecuteAsync(request);

    public async Task RequestStudentLoginCode(RequestStudentLoginCodeRequest request)
        => await _requestStudentLoginCode.ExecuteAsync(request);

    public async Task<AuthenticationResponse> RefreshSession(RefreshSessionRequest request)
        => await _refreshSession.ExecuteAsync(request);

    public async Task LogoutUser(LogoutUserRequest request)
        => await _logoutUser.ExecuteAsync(request);
}
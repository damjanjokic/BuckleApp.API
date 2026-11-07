using BuckleApp.API.Requests.Auth;
using BuckleApp.Application.Features.Authentication.LoginUser;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Application.Features.Authentication.ResetPassword;

namespace BuckleApp.Application.Features.Authentication;

public class AuthService : IAuthService
{
    private readonly ILoginUserService _loginUserService;
    private readonly IRegisterUserService _registerUserService;
    private readonly IResetPasswordService _resetPasswordService;

    public AuthService(ILoginUserService loginUserService, IRegisterUserService registerUserService, IResetPasswordService resetPasswordService)
    {
        _loginUserService = loginUserService;
        _registerUserService = registerUserService;
        _resetPasswordService = resetPasswordService;
    }

    public async Task<string> LoginUser(LoginUserRequest request)
        => await _loginUserService.ExecuteAsync(request);

    public async Task<Guid> RegisterUser(RegisterUserRequest request)
        => await _registerUserService.ExecuteAsync(request);

    public async Task<bool> ResetPassword(ResetPasswordRequest request)
        => await _resetPasswordService.ExecuteAsync(request);
}
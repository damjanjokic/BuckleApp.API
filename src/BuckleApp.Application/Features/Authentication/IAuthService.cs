using BuckleApp.Application.Features.Authentication.ForgotPassword;
using BuckleApp.Application.Features.Authentication.LoginUser;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Application.Features.Authentication.ResetPassword;

namespace BuckleApp.Application.Features.Authentication;

public interface IAuthService
{
    Task<string> LoginUser(LoginUserRequest request);
    Task<Guid> RegisterUser(RegisterUserRequest request);
    Task<bool> ResetPassword(ResetPasswordRequest request);

}
using BuckleApp.Application.Dtos;
using BuckleApp.Application.Features.Authentication.RequestStudentLoginCode;
using BuckleApp.Application.Features.Authentication.RefreshSession;
using BuckleApp.Application.Features.Authentication.LogoutUser;
using BuckleApp.Application.Features.Authentication.ForgotPassword;
using BuckleApp.Application.Features.Authentication.LoginUser;
using BuckleApp.Application.Features.Authentication.RegisterUser;
using BuckleApp.Application.Features.Authentication.ResetPassword;

namespace BuckleApp.Application.Features.Authentication;

public interface IAuthService
{
    Task<AuthenticationResponse> LoginUser(LoginUserRequest request);
    Task<Guid> RegisterUser(RegisterUserRequest request);
    Task<bool> ResetPassword(ResetPasswordRequest request);

    Task RequestStudentLoginCode(RequestStudentLoginCodeRequest request);
    Task<AuthenticationResponse> RefreshSession(RefreshSessionRequest request);
    Task LogoutUser(LogoutUserRequest request);
}
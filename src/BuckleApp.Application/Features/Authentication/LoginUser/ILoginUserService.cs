using BuckleApp.Application.Dtos;

namespace BuckleApp.Application.Features.Authentication.LoginUser;

public interface ILoginUserService
{
    Task<AuthenticationResponse> ExecuteAsync(LoginUserRequest request);
}
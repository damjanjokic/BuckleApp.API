namespace BuckleApp.Application.Features.Authentication.LoginUser;

public interface ILoginUserService
{
    Task<string> ExecuteAsync(LoginUserRequest request);
}
namespace BuckleApp.Application.Features.Authentication.LogoutUser;

public interface ILogoutUserService
{
    Task ExecuteAsync(LogoutUserRequest request);
}

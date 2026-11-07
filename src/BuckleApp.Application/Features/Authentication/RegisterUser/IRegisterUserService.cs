using BuckleApp.Application.Features.Authentication.RegisterUser;

namespace BuckleApp.API.Requests.Auth;

public interface IRegisterUserService
{
    Task<Guid> ExecuteAsync(RegisterUserRequest request);
}
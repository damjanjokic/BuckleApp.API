using BuckleApp.Application.Dtos;

namespace BuckleApp.Application.Features.Authentication.RefreshSession;

public interface IRefreshSessionService
{
    Task<AuthenticationResponse> ExecuteAsync(RefreshSessionRequest request);
}

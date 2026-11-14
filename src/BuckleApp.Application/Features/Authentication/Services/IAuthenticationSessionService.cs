using BuckleApp.Application.Dtos;
using BuckleApp.Core.Entities;

namespace BuckleApp.Application.Features.Authentication.Services;

public interface IAuthenticationSessionService
{
    /// <summary>
    /// Stages session credentials inside the caller's transaction without saving or committing.
    /// Omit expiresAt for a new session; pass its original expiry when rotating refresh tokens.
    /// </summary>
    Task<AuthenticationResponse> CreateAsync(User user, Guid familyId, DateTime? expiresAt = null);
}

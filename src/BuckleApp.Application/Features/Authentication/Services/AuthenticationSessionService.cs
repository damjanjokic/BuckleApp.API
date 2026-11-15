using Microsoft.Extensions.Logging;
using BuckleApp.Infrastructure.Security;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BuckleApp.Application.Dtos;
using BuckleApp.Core.Entities;
using BuckleApp.Core.IRepositories;
using BuckleApp.Infrastructure.Interfaces;

namespace BuckleApp.Application.Features.Authentication.Services;

public class AuthenticationSessionService : IAuthenticationSessionService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly AuthenticationOptions _options;
    private readonly ILogger<AuthenticationSessionService> _logger;

    public AuthenticationSessionService(IAuthRepository authRepository, IJwtGenerator jwtGenerator, IOptions<AuthenticationOptions> options, ILogger<AuthenticationSessionService> logger)
    {
        _authRepository = authRepository;
        _jwtGenerator = jwtGenerator;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AuthenticationResponse> CreateAsync(User user, Guid familyId, DateTime? expiresAt = null)
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var now = DateTime.UtcNow;
        var refreshExpiresAt = expiresAt ?? now.AddDays(_options.RefreshSessionLifetimeDays);
        // JWT NumericDate represents whole seconds; return the same instant to clients.
        var accessExpiresAt = DateTimeOffset.FromUnixTimeSeconds(
            new DateTimeOffset(now.AddMinutes(_options.AccessTokenLifetimeMinutes)).ToUnixTimeSeconds()).UtcDateTime;
        var roles = (await _authRepository.GetUserRoles(user)).ToList();
        var access = _jwtGenerator.CreateToken(user, roles, accessExpiresAt);
        _authRepository.AddRefreshToken(new RefreshToken { Id = Guid.NewGuid(), UserId = user.Id, FamilyId = familyId,
            TokenHash = AuthenticationRules.HashRefreshToken(raw), SecurityStamp = user.SecurityStamp ?? "", CreatedAt = now, ExpiresAt = refreshExpiresAt });
        _logger.LogDebug("Session credentials staged for user {UserId} in session {SessionId}", user.Id, familyId);
        return new(access, raw, accessExpiresAt, refreshExpiresAt);
    }
}

namespace BuckleApp.Application.Dtos;

public sealed record AuthenticationResponse(string AccessToken, string RefreshToken,
    DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);

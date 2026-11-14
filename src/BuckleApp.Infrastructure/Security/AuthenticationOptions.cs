using System.ComponentModel.DataAnnotations;

namespace BuckleApp.Infrastructure.Security;

public class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    [Range(1, 1440)]
    public int AccessTokenLifetimeMinutes { get; set; }

    [Range(1, 365)]
    public int RefreshSessionLifetimeDays { get; set; }

    [Range(1, 60)]
    public int LoginCodeLifetimeMinutes { get; set; }

    [Range(1, 1440)]
    public int IdentityTokenLifetimeMinutes { get; set; }
}

using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace BuckleApp.API.Extensions;

public static class RateLimiterConfiguration
{
    public static void ConfigureRateLimiter(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("authentication", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 20,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
        });
    }
}

using System.Security.Claims;
using System.Text;
using BuckleApp.Core.IRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BuckleApp.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BuckleApp.API.Extensions;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthenticationOptions(this WebApplicationBuilder builder)
    {
        var codeKey = Convert.FromBase64String(builder.Configuration["Authentication:LoginCodeHashKey"]
            ?? throw new InvalidOperationException("Configure Authentication:LoginCodeHashKey with at least 32 random bytes (Base64)."));
        if (codeKey.Length < 32) throw new InvalidOperationException("Login code hash key is too short.");

        builder.Services.AddOptions<AuthenticationOptions>()
            .Bind(builder.Configuration.GetSection(AuthenticationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<DataProtectionTokenProviderOptions>()
            .Configure<IOptions<AuthenticationOptions>>((identity, authentication) =>
                identity.TokenLifespan = TimeSpan.FromMinutes(authentication.Value.IdentityTokenLifetimeMinutes));
    }

    public static void ConfigureJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var id = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var auth = context.HttpContext.RequestServices.GetRequiredService<IAuthRepository>();
                        if (!Guid.TryParse(id, out var userId))
                        {
                            context.Fail("Invalid credentials");
                            return;
                        }
                        var user = await auth.FindByIdAsync(userId);
                        if (user == null || user.isDeleted || user.IsDisabled || user.LockoutEnd > DateTimeOffset.UtcNow)
                            context.Fail("Invalid credentials");
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }
}

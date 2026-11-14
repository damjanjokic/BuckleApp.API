using BuckleApp.Core.Entities;
using BuckleApp.Data.Data;
using Microsoft.AspNetCore.Identity;

namespace BuckleApp.API.Extensions;

public static class IdentityConfiguration
{
    public static void ConfigureIdentity(this WebApplicationBuilder builder) =>
        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

    // public static void ConfigureIdentityOps(this WebApplicationBuilder builder)
    // {
    //     var identity = builder.Services.AddIdentityCore<User>(opt =>
    //     {
    //         opt.Password.RequireDigit = true;
    //         opt.Password.RequiredLength = 6;
    //         opt.Password.RequireNonAlphanumeric = false;
    //         opt.Password.RequireUppercase = true;
    //         opt.SignIn.RequireConfirmedEmail = false;
    //     });
    // }
}
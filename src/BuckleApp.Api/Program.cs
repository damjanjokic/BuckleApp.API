using BuckleApp.API.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.ConfigureIdentity();
builder.ConfigureSqlContext();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await SeedRolesAsync(roleManager);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred seeding roles: {ex.Message}");
    }
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
{
    string[] roleNames = { "Admin", "SuperAdmin", "Instructor", "Student" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }
    }
}
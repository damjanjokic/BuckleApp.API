using BuckleApp.API.Extensions;
using BuckleApp.API.Middleware;
using BuckleApp.Application.Mapping;
using BuckleApp.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureRateLimiter();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ResolveDependenciesExtension();
builder.ConfigureIdentity();
builder.ConfigureAuthenticationOptions();
builder.ConfigureSqlContext();

MapsterConfig.RegisterMappings();

builder.ConfigureJwtAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var resendGridConfig =
    builder.Configuration.GetSection("Resend");
builder.Services.Configure<SmtpConfig>(resendGridConfig);

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
        // Log or handle exceptions
        Console.WriteLine($"Error occurred seeding roles: {ex.Message}");
    }

    // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    // dbContext.Database.Migrate();
}

app.UseCors("AllowSpecificOrigins");
app.UseMiddleware<ErrorHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

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
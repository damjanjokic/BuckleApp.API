using System.Text;
using BuckleApp.API.Extensions;
using BuckleApp.API.Middleware;
using BuckleApp.Application.Mapping;
using BuckleApp.Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.ResolveDependenciesExtension();
builder.ConfigureIdentity();
builder.ConfigureIdentityToken();
builder.ConfigureSqlContext();

MapsterConfig.RegisterMappings();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                .GetBytes(builder.Configuration.GetSection("Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
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
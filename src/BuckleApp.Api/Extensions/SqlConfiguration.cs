using BuckleApp.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace BuckleApp.API.Extensions;

public static class SqlConfiguration
{
    public static void ConfigureSqlContext(this WebApplicationBuilder builder) =>
        builder.Services.AddDbContext<ApplicationContext>(opts =>
            opts.UseNpgsql(builder.Configuration.GetConnectionString("sqlConnection")));
}
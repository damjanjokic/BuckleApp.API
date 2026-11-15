using System.Diagnostics;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace BuckleApp.API.Extensions;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Services.AddSerilog((services, logger) =>
        {
            logger.ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "BuckleApp.Api")
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);

            // Development file sinks are configured in appsettings.Development.json.
            // Keep console logging available in other environments as well.
            if (!builder.Configuration.GetSection("Serilog:WriteTo").Exists())
                logger.WriteTo.Console(new CompactJsonFormatter());
        });
    }

    public static void UseRequestLogging(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
            using (app.Logger.BeginScope(new Dictionary<string, object>
            {
                ["TraceId"] = traceId,
                ["RequestId"] = context.TraceIdentifier
            }))
            {
                await next(context);
            }
        });

        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (context, _, exception) => exception != null || context.Response.StatusCode >= 500
                ? LogEventLevel.Error
                : context.Response.StatusCode >= 400 ? LogEventLevel.Warning : LogEventLevel.Information;
            options.EnrichDiagnosticContext = (diagnostics, context) =>
            {
                diagnostics.Set("TraceId", Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier);
                diagnostics.Set("RequestId", context.TraceIdentifier);
            };
        });
    }
}

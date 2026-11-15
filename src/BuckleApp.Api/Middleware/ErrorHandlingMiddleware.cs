using System.Net;
using System.Text.Json;
using BuckleApp.Application.Exceptions;

namespace BuckleApp.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlingMiddleware> logger)
    {
        object errors = null;

        switch (ex)
        {
            case RestException re:
                if ((int)re.Code >= 500)
                    logger.LogError(ex, "Application operation failed with status {StatusCode}", (int)re.Code);
                else
                    logger.LogWarning("Request rejected with status {StatusCode}", (int)re.Code);
                errors = re.Errors;
                context.Response.StatusCode = (int)re.Code;
                break;
            case Exception e:
                logger.LogError(ex, "Unhandled request exception");
                errors = "An unexpected error occurred";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";
        if (errors != null)
        {
            var result = JsonSerializer.Serialize(new
            {
                errors
            });

            await context.Response.WriteAsync(result);
        }
    }
}
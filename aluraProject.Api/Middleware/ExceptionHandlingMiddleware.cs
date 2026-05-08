using System.Text.Json;
using aluraProject.Application.Common.Exceptions;

namespace aluraProject.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, ex);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var status = StatusCodes.Status500InternalServerError;
        var message = "An unexpected error occurred.";

        if (exception is ApiException apiException)
        {
            status = apiException.StatusCode;
            message = apiException.Message;
        }
        else if (exception is ArgumentException or FormatException)
        {
            status = StatusCodes.Status400BadRequest;
            message = exception.Message;
        }

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status,
            message,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

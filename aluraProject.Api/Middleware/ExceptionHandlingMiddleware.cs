using aluraProject.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

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
            var (status, _, _) = ResolveError(ex);

            if (status >= StatusCodes.Status500InternalServerError)
            {
                logger.LogError(
                    ex,
                    "Unhandled exception. Status: {StatusCode}. Path: {Path}. Method: {Method}. TraceId: {TraceId}",
                    status,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier);
            }
            else
            {
                logger.LogWarning(
                    "Handled exception. Status: {StatusCode}. Path: {Path}. Method: {Method}. TraceId: {TraceId}. Message: {Message}",
                    status,
                    context.Request.Path,
                    context.Request.Method,
                    context.TraceIdentifier,
                    ex.Message);
            }

            await WriteErrorAsync(context, ex);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var (status, message, errorCode) = ResolveError(exception);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title = ReasonPhrases.GetReasonPhrase(status),
            Detail = message,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["timestampUtc"] = DateTime.UtcNow;
        problem.Extensions["errorCode"] = errorCode;

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static (int Status, string Message, string ErrorCode) ResolveError(Exception exception)
    {
        if (exception is ApiException apiException)
        {
            return (apiException.StatusCode, apiException.Message, apiException.ErrorCode);
        }

        if (exception is ArgumentException or FormatException)
        {
            return (StatusCodes.Status400BadRequest, exception.Message, "validation_error");
        }

        return (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "internal_error");
    }
}

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace aluraProject.Api.Swagger;

public sealed class SwaggerExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var relativePath = context.ApiDescription.RelativePath?.ToLowerInvariant() ?? string.Empty;
        var method = context.ApiDescription.HttpMethod?.ToUpperInvariant() ?? string.Empty;

        AddRequestExamples(operation, relativePath, method);
        AddResponseExamples(operation, relativePath, method);
        AddStandardErrorExamples(operation);
    }

    private static void AddRequestExamples(OpenApiOperation operation, string relativePath, string method)
    {
        var mediaType = GetJsonRequestMediaType(operation);
        if (mediaType is null)
        {
            return;
        }

        if (relativePath == "api/auth/register" && method == "POST")
        {
            mediaType.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("student@aluraproject.local"),
                ["password"] = new OpenApiString("StrongPass#123"),
                ["role"] = new OpenApiString("Student")
            };
            return;
        }

        if (relativePath == "api/auth/login" && method == "POST")
        {
            mediaType.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("student@aluraproject.local"),
                ["password"] = new OpenApiString("StrongPass#123")
            };
            return;
        }

        if ((relativePath == "api/enrollments" || relativePath == "enrollments") && method == "POST")
        {
            mediaType.Example = new OpenApiObject
            {
                ["courseId"] = new OpenApiString("9cb3f40c-ccfd-4daf-9f0f-64f0d180fe67"),
                ["studentId"] = new OpenApiString("f45ea894-3a41-4479-95ec-1945404b72bf")
            };
        }
    }

    private static void AddResponseExamples(OpenApiOperation operation, string relativePath, string method)
    {
        if (relativePath == "api/auth/login" && method == "POST")
        {
            SetJsonResponseExample(
                operation,
                "200",
                new OpenApiObject
                {
                    ["accessToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."),
                    ["expiresAtUtc"] = new OpenApiString("2026-05-20T22:00:00Z"),
                    ["email"] = new OpenApiString("student@aluraproject.local"),
                    ["role"] = new OpenApiString("Student")
                });
            return;
        }

        if ((relativePath == "api/enrollments" || relativePath == "enrollments") && method == "POST")
        {
            SetJsonResponseExample(
                operation,
                "201",
                new OpenApiObject
                {
                    ["id"] = new OpenApiString("f1ec42f0-3220-4ef4-b11b-b851f9b4f9cf"),
                    ["courseId"] = new OpenApiString("9cb3f40c-ccfd-4daf-9f0f-64f0d180fe67"),
                    ["courseTitle"] = new OpenApiString("API REST com ASP.NET Core"),
                    ["studentId"] = new OpenApiString("f45ea894-3a41-4479-95ec-1945404b72bf"),
                    ["status"] = new OpenApiString("Active"),
                    ["enrolledAtUtc"] = new OpenApiString("2026-05-20T20:00:00Z")
                });
            return;
        }

        if ((relativePath == "students/{id}/enrollments" || relativePath == "api/students/{id}/enrollments") && method == "GET")
        {
            SetJsonResponseExample(
                operation,
                "200",
                new OpenApiObject
                {
                    ["items"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["id"] = new OpenApiString("f1ec42f0-3220-4ef4-b11b-b851f9b4f9cf"),
                            ["courseId"] = new OpenApiString("9cb3f40c-ccfd-4daf-9f0f-64f0d180fe67"),
                            ["courseTitle"] = new OpenApiString("API REST com ASP.NET Core"),
                            ["studentId"] = new OpenApiString("f45ea894-3a41-4479-95ec-1945404b72bf"),
                            ["status"] = new OpenApiString("Active"),
                            ["enrolledAtUtc"] = new OpenApiString("2026-05-20T20:00:00Z")
                        }
                    },
                    ["page"] = new OpenApiInteger(1),
                    ["pageSize"] = new OpenApiInteger(10),
                    ["totalItems"] = new OpenApiInteger(1),
                    ["totalPages"] = new OpenApiInteger(1)
                });
        }
    }

    private static void AddStandardErrorExamples(OpenApiOperation operation)
    {
        AddProblemExample(
            operation,
            "400",
            "Bad Request",
            "validation_error",
            "One or more validation errors occurred.");
        AddProblemExample(operation, "401", "Unauthorized", "unauthorized", "Authenticated user id not found.");
        AddProblemExample(operation, "403", "Forbidden", "forbidden", "You can only access your own student profile.");
        AddProblemExample(operation, "404", "Not Found", "not_found", "Student not found.");
        AddProblemExample(operation, "409", "Conflict", "conflict", "Student is already enrolled in this course.");
        AddProblemExample(
            operation,
            "422",
            "Unprocessable Entity",
            "business_rule_violation",
            "Course does not exist or is inactive.");
    }

    private static void AddProblemExample(
        OpenApiOperation operation,
        string statusCode,
        string title,
        string errorCode,
        string detail)
    {
        if (!operation.Responses.TryGetValue(statusCode, out var response))
        {
            return;
        }

        if (!response.Content.TryGetValue("application/problem+json", out var content)
            && !response.Content.TryGetValue("application/json", out content))
        {
            return;
        }

        content.Example = new OpenApiObject
        {
            ["status"] = new OpenApiInteger(int.Parse(statusCode)),
            ["title"] = new OpenApiString(title),
            ["detail"] = new OpenApiString(detail),
            ["instance"] = new OpenApiString("/api/example"),
            ["traceId"] = new OpenApiString("00-a1b2c3d4e5f60789abcd1234567890ab-0123456789abcdef-00"),
            ["timestampUtc"] = new OpenApiString("2026-05-20T20:10:00Z"),
            ["errorCode"] = new OpenApiString(errorCode)
        };
    }

    private static OpenApiMediaType? GetJsonRequestMediaType(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content is null)
        {
            return null;
        }

        if (operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
        {
            return mediaType;
        }

        return null;
    }

    private static void SetJsonResponseExample(OpenApiOperation operation, string statusCode, IOpenApiAny example)
    {
        if (!operation.Responses.TryGetValue(statusCode, out var response))
        {
            return;
        }

        if (response.Content.TryGetValue("application/json", out var content))
        {
            content.Example = example;
        }
    }
}

namespace aluraProject.Application.Common.Exceptions;

public class ApiException : Exception
{
    protected ApiException(string message, int statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public int StatusCode { get; }
    public string ErrorCode { get; }
}

public sealed class ValidationException(string message)
    : ApiException(message, 400, "validation_error");

public sealed class UnauthorizedException(string message)
    : ApiException(message, 401, "unauthorized");

public sealed class ForbiddenException(string message)
    : ApiException(message, 403, "forbidden");

public sealed class NotFoundException(string message)
    : ApiException(message, 404, "not_found");

public sealed class ConflictException(string message)
    : ApiException(message, 409, "conflict");

public sealed class BusinessRuleViolationException(string message)
    : ApiException(message, 422, "business_rule_violation");

namespace aluraProject.Application.Common.Exceptions;

public class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public sealed class ValidationException(string message) : ApiException(message, 400);
public sealed class UnauthorizedException(string message) : ApiException(message, 401);
public sealed class ForbiddenException(string message) : ApiException(message, 403);
public sealed class NotFoundException(string message) : ApiException(message, 404);
public sealed class ConflictException(string message) : ApiException(message, 409);

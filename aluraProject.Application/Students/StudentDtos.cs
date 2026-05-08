namespace aluraProject.Application.Students;

public sealed record CreateStudentRequest(string UserId, string FullName, string Email);
public sealed record UpdateStudentRequest(string FullName, string Email);

public sealed record StudentResponse(
    Guid Id,
    string UserId,
    string FullName,
    string Email,
    DateTime RegisteredAtUtc,
    DateTime UpdatedAtUtc,
    bool IsDeleted);

public sealed record StudentQuery(int Page = 1, int PageSize = 10, string? Email = null);

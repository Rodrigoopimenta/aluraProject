using System.ComponentModel.DataAnnotations;

namespace aluraProject.Application.Students;

public sealed record CreateStudentRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MaxLength(450)]
    string UserId,
    [property: Required(AllowEmptyStrings = false)]
    [property: MinLength(3)]
    [property: MaxLength(200)]
    string FullName,
    [property: Required(AllowEmptyStrings = false)]
    [property: EmailAddress]
    [property: MaxLength(256)]
    string Email);

public sealed record UpdateStudentRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MinLength(3)]
    [property: MaxLength(200)]
    string FullName,
    [property: Required(AllowEmptyStrings = false)]
    [property: EmailAddress]
    [property: MaxLength(256)]
    string Email);

public sealed record StudentResponse(
    Guid Id,
    string UserId,
    string FullName,
    string Email,
    DateTime RegisteredAtUtc,
    DateTime UpdatedAtUtc,
    bool IsDeleted);

public sealed record StudentQuery(int Page = 1, int PageSize = 10, string? Email = null);

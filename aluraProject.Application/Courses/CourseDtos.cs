using System.ComponentModel.DataAnnotations;

namespace aluraProject.Application.Courses;

public sealed record CreateCourseRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MinLength(3)]
    [property: MaxLength(200)]
    string Title,
    [property: MaxLength(1000)]
    string? Description,
    [property: Required(AllowEmptyStrings = false)]
    [property: MinLength(2)]
    [property: MaxLength(80)]
    string Category,
    [property: Range(1, 2000)]
    int WorkloadHours);

public sealed record UpdateCourseRequest(
    [property: Required(AllowEmptyStrings = false)]
    [property: MinLength(3)]
    [property: MaxLength(200)]
    string Title,
    [property: MaxLength(1000)]
    string? Description,
    [property: Required(AllowEmptyStrings = false)]
    [property: MinLength(2)]
    [property: MaxLength(80)]
    string Category,
    [property: Range(1, 2000)]
    int WorkloadHours);

public sealed record CourseResponse(
    Guid Id,
    string Title,
    string? Description,
    string Category,
    int WorkloadHours,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record CourseQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? Category = null,
    string SortBy = "createdAt",
    string SortOrder = "desc");

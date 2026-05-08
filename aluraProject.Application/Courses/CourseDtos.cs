namespace aluraProject.Application.Courses;

public sealed record CreateCourseRequest(string Title, string? Description, string Category, int WorkloadHours);
public sealed record UpdateCourseRequest(string Title, string? Description, string Category, int WorkloadHours);

public sealed record CourseResponse(
    Guid Id,
    string Title,
    string? Description,
    string Category,
    int WorkloadHours,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record CourseQuery(int Page = 1, int PageSize = 10, string? Title = null, string? Category = null);

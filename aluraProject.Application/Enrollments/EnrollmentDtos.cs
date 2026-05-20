using aluraProject.Application.Common.Validation;
using aluraProject.Domain.Entities;

namespace aluraProject.Application.Enrollments;

public sealed record CreateEnrollmentRequest(
    [property: NotEmptyGuid]
    Guid CourseId,
    [property: NotEmptyGuid]
    Guid? StudentId = null);

public sealed record EnrollmentQuery(int Page = 1, int PageSize = 10, EnrollmentStatus? Status = null);

public sealed record EnrollmentResponse(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    Guid StudentId,
    EnrollmentStatus Status,
    DateTime EnrolledAtUtc);

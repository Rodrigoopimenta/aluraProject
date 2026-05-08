using aluraProject.Domain.Entities;

namespace aluraProject.Application.Enrollments;

public sealed record CreateEnrollmentRequest(Guid CourseId);

public sealed record EnrollmentResponse(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    Guid StudentId,
    EnrollmentStatus Status,
    DateTime EnrolledAtUtc);

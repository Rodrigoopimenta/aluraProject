using aluraProject.Application.Common;
using aluraProject.Application.Enrollments;

namespace aluraProject.Application.Abstractions.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponse> EnrollAsync(
        string authenticatedUserId,
        bool isAdmin,
        CreateEnrollmentRequest request,
        CancellationToken cancellationToken);

    Task<PagedResult<EnrollmentResponse>> ListByStudentAsync(
        Guid studentId,
        EnrollmentQuery query,
        CancellationToken cancellationToken);
}

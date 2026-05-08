using aluraProject.Application.Enrollments;

namespace aluraProject.Application.Abstractions.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponse> EnrollAsync(string authenticatedUserId, CreateEnrollmentRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<EnrollmentResponse>> ListByUserAsync(string authenticatedUserId, bool isAdmin, CancellationToken cancellationToken);
}

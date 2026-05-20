using aluraProject.Application.Common;
using aluraProject.Domain.Entities;

namespace aluraProject.Application.Abstractions.Repositories;

public interface IEnrollmentRepository
{
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
    Task<PagedResult<Enrollment>> ListByStudentAsync(
        Guid studentId,
        int page,
        int pageSize,
        EnrollmentStatus? status,
        CancellationToken cancellationToken);
}

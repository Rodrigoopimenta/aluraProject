using aluraProject.Domain.Entities;

namespace aluraProject.Application.Abstractions.Repositories;

public interface IEnrollmentRepository
{
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Enrollment>> ListByStudentAsync(Guid studentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Enrollment>> ListAllAsync(CancellationToken cancellationToken);
}

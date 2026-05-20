using aluraProject.Application.Common;
using aluraProject.Domain.Entities;

namespace aluraProject.Application.Abstractions.Repositories;

public interface IStudentRepository
{
    Task AddAsync(Student student, CancellationToken cancellationToken);
    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Student?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<PagedResult<Student>> ListAsync(int page, int pageSize, string? emailFilter, CancellationToken cancellationToken);
}

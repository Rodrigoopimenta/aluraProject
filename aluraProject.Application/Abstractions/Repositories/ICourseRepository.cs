using aluraProject.Application.Common;
using aluraProject.Domain.Entities;

namespace aluraProject.Application.Abstractions.Repositories;

public interface ICourseRepository
{
    Task AddAsync(Course course, CancellationToken cancellationToken);
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<Course>> ListAsync(int page, int pageSize, string? titleFilter, string? categoryFilter, CancellationToken cancellationToken);
}

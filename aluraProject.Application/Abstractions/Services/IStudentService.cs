using aluraProject.Application.Common;
using aluraProject.Application.Students;

namespace aluraProject.Application.Abstractions.Services;

public interface IStudentService
{
    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken);
    Task<PagedResult<StudentResponse>> ListAsync(StudentQuery query, CancellationToken cancellationToken);
    Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<StudentResponse> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

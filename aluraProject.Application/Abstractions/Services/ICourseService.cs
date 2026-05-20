using aluraProject.Application.Common;
using aluraProject.Application.Courses;

namespace aluraProject.Application.Abstractions.Services;

public interface ICourseService
{
    Task<CourseResponse> CreateAsync(CreateCourseRequest request, string createdByUserId, CancellationToken cancellationToken);
    Task<PagedResult<CourseResponse>> ListAsync(CourseQuery query, CancellationToken cancellationToken);
    Task<CourseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

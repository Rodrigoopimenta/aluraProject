using aluraProject.Application.Abstractions.Repositories;
using aluraProject.Application.Common;
using aluraProject.Domain.Entities;
using aluraProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace aluraProject.Infrastructure.Repositories;

public sealed class EnrollmentRepository(AppDbContext dbContext) : IEnrollmentRepository
{
    public Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken) =>
        dbContext.Enrollments.AddAsync(enrollment, cancellationToken).AsTask();

    public Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken) =>
        dbContext.Enrollments.AnyAsync(x => x.StudentId == studentId && x.CourseId == courseId, cancellationToken);

    public async Task<PagedResult<Enrollment>> ListByStudentAsync(
        Guid studentId,
        int page,
        int pageSize,
        EnrollmentStatus? status,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Enrollments
            .AsNoTracking()
            .Include(x => x.Course)
            .Where(x => x.StudentId == studentId);

        if (status is not null)
        {
            query = query.Where(x => x.Status == status);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.EnrolledAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Enrollment>(items, page, pageSize, totalItems);
    }
}

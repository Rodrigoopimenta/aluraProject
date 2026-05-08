using aluraProject.Application.Abstractions.Repositories;
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

    public async Task<IReadOnlyList<Enrollment>> ListByStudentAsync(Guid studentId, CancellationToken cancellationToken)
    {
        return await dbContext.Enrollments
            .AsNoTracking()
            .Include(x => x.Course)
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.EnrolledAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> ListAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Enrollments
            .AsNoTracking()
            .Include(x => x.Course)
            .Include(x => x.Student)
            .OrderByDescending(x => x.EnrolledAtUtc)
            .ToListAsync(cancellationToken);
    }
}

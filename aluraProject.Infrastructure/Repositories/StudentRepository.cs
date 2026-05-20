using aluraProject.Application.Abstractions.Repositories;
using aluraProject.Application.Common;
using aluraProject.Domain.Entities;
using aluraProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace aluraProject.Infrastructure.Repositories;

public sealed class StudentRepository(AppDbContext dbContext) : IStudentRepository
{
    public Task AddAsync(Student student, CancellationToken cancellationToken) =>
        dbContext.Students.AddAsync(student, cancellationToken).AsTask();

    public Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Students.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Student?> GetByUserIdAsync(string userId, CancellationToken cancellationToken) =>
        dbContext.Students.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        dbContext.Students.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<PagedResult<Student>> ListAsync(int page, int pageSize, string? emailFilter, CancellationToken cancellationToken)
    {
        var query = dbContext.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(emailFilter))
        {
            var normalized = emailFilter.Trim().ToLower();
            query = query.Where(x => x.Email.ToLower().Contains(normalized));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.RegisteredAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Student>(items, page, pageSize, totalItems);
    }
}

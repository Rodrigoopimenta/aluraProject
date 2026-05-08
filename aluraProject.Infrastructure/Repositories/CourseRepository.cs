using aluraProject.Application.Abstractions.Repositories;
using aluraProject.Application.Common;
using aluraProject.Domain.Entities;
using aluraProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace aluraProject.Infrastructure.Repositories;

public sealed class CourseRepository(AppDbContext dbContext) : ICourseRepository
{
    public Task AddAsync(Course course, CancellationToken cancellationToken) =>
        dbContext.Courses.AddAsync(course, cancellationToken).AsTask();

    public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<Course>> ListAsync(
        int page,
        int pageSize,
        string? search,
        string? categoryFilter,
        string sortBy,
        string sortOrder,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(x =>
                x.Title.ToLower().Contains(normalized) ||
                x.Category.ToLower().Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(categoryFilter))
        {
            var normalized = categoryFilter.Trim().ToLower();
            query = query.Where(x => x.Category.ToLower().Contains(normalized));
        }

        var desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy.Trim().ToLowerInvariant() switch
        {
            "title" => desc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
            "category" => desc ? query.OrderByDescending(x => x.Category) : query.OrderBy(x => x.Category),
            "createdat" => desc ? query.OrderByDescending(x => x.CreatedAtUtc) : query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderByDescending(x => x.CreatedAtUtc)
        };

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Course>(items, page, pageSize, totalItems);
    }
}

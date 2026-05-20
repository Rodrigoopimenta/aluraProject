using aluraProject.Application.Abstractions.Repositories;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Courses;
using aluraProject.Domain.Entities;

namespace aluraProject.Application.Services;

public sealed class CourseService(ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICourseService
{
    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request, string createdByUserId, CancellationToken cancellationToken)
    {
        try
        {
            var course = new Course(request.Title, request.Description, request.Category, request.WorkloadHours, createdByUserId);
            await courseRepository.AddAsync(course, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(course);
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    public async Task<PagedResult<CourseResponse>> ListAsync(CourseQuery query, CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : Math.Min(query.PageSize, 100);
        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "createdAt" : query.SortBy.Trim();
        var sortOrder = string.IsNullOrWhiteSpace(query.SortOrder) ? "desc" : query.SortOrder.Trim();
        var data = await courseRepository.ListAsync(page, pageSize, query.Search, query.Category, sortBy, sortOrder, cancellationToken);
        return new PagedResult<CourseResponse>(data.Items.Select(Map).ToList(), data.Page, data.PageSize, data.TotalItems);
    }

    public async Task<CourseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("Course not found.");
        }

        return Map(course);
    }

    public async Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("Course not found.");
        }

        try
        {
            course.Update(request.Title, request.Description, request.Category, request.WorkloadHours);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(course);
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("Course not found.");
        }

        course.SoftDelete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CourseResponse Map(Course course) =>
        new(course.Id, course.Title, course.Description, course.Category, course.WorkloadHours, course.CreatedAtUtc, course.UpdatedAtUtc);
}

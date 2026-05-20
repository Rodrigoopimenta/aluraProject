using aluraProject.Application.Abstractions.Repositories;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Enrollments;

namespace aluraProject.Application.Services;

public sealed class EnrollmentService(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ICourseRepository courseRepository,
    IUnitOfWork unitOfWork) : IEnrollmentService
{
    public async Task<EnrollmentResponse> EnrollAsync(
        string authenticatedUserId,
        bool isAdmin,
        CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.CourseId == Guid.Empty)
        {
            throw new ValidationException("courseId is required.");
        }

        var student = await ResolveStudentAsync(authenticatedUserId, isAdmin, request.StudentId, cancellationToken);

        var course = await courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new BusinessRuleViolationException("Course does not exist or is inactive.");
        }

        var exists = await enrollmentRepository.ExistsAsync(student.Id, request.CourseId, cancellationToken);
        if (exists)
        {
            throw new ConflictException("Student is already enrolled in this course.");
        }

        var enrollment = new Domain.Entities.Enrollment(student.Id, request.CourseId);
        await enrollmentRepository.AddAsync(enrollment, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (IsUniqueEnrollmentViolation(ex))
        {
            throw new ConflictException("Student is already enrolled in this course.");
        }

        return new EnrollmentResponse(
            enrollment.Id,
            course.Id,
            course.Title,
            student.Id,
            enrollment.Status,
            enrollment.EnrolledAtUtc);
    }

    public async Task<PagedResult<EnrollmentResponse>> ListByStudentAsync(
        Guid studentId,
        EnrollmentQuery query,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(studentId, cancellationToken);
        if (student is null)
        {
            throw new NotFoundException("Student not found.");
        }

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : Math.Min(query.PageSize, 100);

        var data = await enrollmentRepository.ListByStudentAsync(
            studentId,
            page,
            pageSize,
            query.Status,
            cancellationToken);

        var items = data.Items
            .Select(e => new EnrollmentResponse(
                e.Id,
                e.CourseId,
                e.Course?.Title ?? string.Empty,
                e.StudentId,
                e.Status,
                e.EnrolledAtUtc))
            .ToList();

        return new PagedResult<EnrollmentResponse>(items, data.Page, data.PageSize, data.TotalItems);
    }

    private async Task<Domain.Entities.Student> ResolveStudentAsync(
        string authenticatedUserId,
        bool isAdmin,
        Guid? payloadStudentId,
        CancellationToken cancellationToken)
    {
        if (isAdmin)
        {
            if (payloadStudentId is null || payloadStudentId == Guid.Empty)
            {
                throw new ValidationException("studentId is required when an admin creates an enrollment.");
            }

            var adminSelectedStudent = await studentRepository.GetByIdAsync(payloadStudentId.Value, cancellationToken);
            if (adminSelectedStudent is null)
            {
                throw new BusinessRuleViolationException("Student does not exist or is inactive.");
            }

            return adminSelectedStudent;
        }

        if (payloadStudentId is not null)
        {
            throw new ValidationException("studentId cannot be sent by student users.");
        }

        var currentStudent = await studentRepository.GetByUserIdAsync(authenticatedUserId, cancellationToken);
        if (currentStudent is null)
        {
            throw new BusinessRuleViolationException("Student profile does not exist or is inactive for the authenticated user.");
        }

        return currentStudent;
    }

    private static bool IsUniqueEnrollmentViolation(Exception exception)
    {
        Exception? current = exception;
        while (current is not null)
        {
            if (current.GetType().Name.Contains("SqliteException", StringComparison.OrdinalIgnoreCase)
                && current.Message.Contains("Enrollments.StudentId, Enrollments.CourseId", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            current = current.InnerException;
        }

        return false;
    }
}

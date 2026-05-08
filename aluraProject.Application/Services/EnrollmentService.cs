using aluraProject.Application.Abstractions.Repositories;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Enrollments;
using aluraProject.Domain.Entities;

namespace aluraProject.Application.Services;

public sealed class EnrollmentService(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    ICourseRepository courseRepository,
    IUnitOfWork unitOfWork) : IEnrollmentService
{
    public async Task<EnrollmentResponse> EnrollAsync(string authenticatedUserId, CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByUserIdAsync(authenticatedUserId, cancellationToken);
        if (student is null)
        {
            throw new NotFoundException("Student profile not found for authenticated user.");
        }

        var course = await courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("Course not found.");
        }

        var exists = await enrollmentRepository.ExistsAsync(student.Id, request.CourseId, cancellationToken);
        if (exists)
        {
            throw new ConflictException("Student is already enrolled in this course.");
        }

        var enrollment = new Enrollment(student.Id, request.CourseId);
        await enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EnrollmentResponse(enrollment.Id, course.Id, course.Title, student.Id, enrollment.Status, enrollment.EnrolledAtUtc);
    }

    public async Task<IReadOnlyList<EnrollmentResponse>> ListByUserAsync(string authenticatedUserId, bool isAdmin, CancellationToken cancellationToken)
    {
        IReadOnlyList<Enrollment> enrollments;
        if (isAdmin)
        {
            enrollments = await enrollmentRepository.ListAllAsync(cancellationToken);
        }
        else
        {
            var student = await studentRepository.GetByUserIdAsync(authenticatedUserId, cancellationToken);
            if (student is null)
            {
                throw new NotFoundException("Student profile not found for authenticated user.");
            }

            enrollments = await enrollmentRepository.ListByStudentAsync(student.Id, cancellationToken);
        }

        return enrollments
            .Select(e => new EnrollmentResponse(
                e.Id,
                e.CourseId,
                e.Course?.Title ?? string.Empty,
                e.StudentId,
                e.Status,
                e.EnrolledAtUtc))
            .ToList();
    }
}

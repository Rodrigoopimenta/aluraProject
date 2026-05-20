using aluraProject.Application.Abstractions;
using aluraProject.Application.Abstractions.Repositories;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Students;
using aluraProject.Domain.Entities;

namespace aluraProject.Application.Services;

public sealed class StudentService(
    IStudentRepository studentRepository,
    IIdentityService identityService,
    IUnitOfWork unitOfWork) : IStudentService
{
    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        if (!await identityService.UserExistsAsync(request.UserId, cancellationToken))
        {
            throw new BusinessRuleViolationException("Provided UserId does not exist in Identity.");
        }

        var existingUser = await studentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException("There is already a student profile for this UserId.");
        }

        var existingEmail = await studentRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (existingEmail is not null)
        {
            throw new ConflictException("Student email already exists.");
        }

        try
        {
            var student = new Student(request.UserId, request.FullName, request.Email);
            await studentRepository.AddAsync(student, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(student);
        }
        catch (Exception ex) when (IsUniqueStudentViolation(ex))
        {
            throw new ConflictException("Student email or user id already exists.");
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(ex.Message);
        }
        catch (FormatException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    public async Task<PagedResult<StudentResponse>> ListAsync(StudentQuery query, CancellationToken cancellationToken)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : Math.Min(query.PageSize, 100);
        var data = await studentRepository.ListAsync(page, pageSize, query.Email, cancellationToken);
        return new PagedResult<StudentResponse>(data.Items.Select(Map).ToList(), data.Page, data.PageSize, data.TotalItems);
    }

    public async Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            throw new NotFoundException("Student not found.");
        }

        return Map(student);
    }

    public async Task<StudentResponse> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            throw new NotFoundException("Student not found for current user.");
        }

        return Map(student);
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            throw new NotFoundException("Student not found.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingEmail = await studentRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingEmail is not null && existingEmail.Id != id)
        {
            throw new ConflictException("Student email already exists.");
        }

        try
        {
            student.Update(request.FullName, request.Email);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(student);
        }
        catch (Exception ex) when (IsUniqueStudentViolation(ex))
        {
            throw new ConflictException("Student email already exists.");
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(ex.Message);
        }
        catch (FormatException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            throw new NotFoundException("Student not found.");
        }

        student.SoftDelete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            throw new NotFoundException("Student not found.");
        }

        student.SoftDelete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static StudentResponse Map(Student student) =>
        new(student.Id, student.UserId, student.FullName, student.Email, student.RegisteredAtUtc, student.UpdatedAtUtc, student.IsDeleted);

    private static bool IsUniqueStudentViolation(Exception exception)
    {
        Exception? current = exception;
        while (current is not null)
        {
            if (current.GetType().Name.Contains("SqliteException", StringComparison.OrdinalIgnoreCase)
                && (current.Message.Contains("Students.Email", StringComparison.OrdinalIgnoreCase)
                    || current.Message.Contains("Students.UserId", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            current = current.InnerException;
        }

        return false;
    }
}

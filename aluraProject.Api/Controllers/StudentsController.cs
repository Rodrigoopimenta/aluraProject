using aluraProject.Application.Abstractions;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Enrollments;
using aluraProject.Application.Students;
using aluraProject.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aluraProject.Api.Controllers;

/// <summary>
/// Student profile and enrollment query endpoints.
/// </summary>
[ApiController]
[Route("students")]
[Route("api/students")]
[Authorize]
public sealed class StudentsController(
    IStudentService studentService,
    IEnrollmentService enrollmentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Creates a student profile linked to an existing Identity user.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Role allowed: Admin.
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var created = await studentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Lists students with pagination.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Role allowed: Admin.
    /// Pagination: page starts at 1 and pageSize is limited to 100.
    /// </remarks>
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType<PagedResult<StudentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public Task<PagedResult<StudentResponse>> List([FromQuery] StudentQuery query, CancellationToken cancellationToken) =>
        studentService.ListAsync(query, cancellationToken);

    /// <summary>
    /// Gets the student profile for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Roles allowed: Admin, Student.
    /// </remarks>
    [HttpGet("me")]
    [HttpGet("/me")]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<StudentResponse> GetMe(CancellationToken cancellationToken)
    {
        var userId = EnsureAuthenticatedUserId();
        return studentService.GetByUserIdAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Gets a student by identifier.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Roles allowed: Admin or resource owner.
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<StudentResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentService.GetByIdAsync(id, cancellationToken);
        EnforceAdminOrOwner(student.UserId);
        return student;
    }

    /// <summary>
    /// Lists enrollments for a student.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Roles allowed: Admin or resource owner.
    /// Pagination: page starts at 1 and pageSize is limited to 100.
    /// Filter: status (Active, Cancelled).
    /// </remarks>
    [HttpGet("{id:guid}/enrollments")]
    [ProducesResponseType<PagedResult<EnrollmentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<PagedResult<EnrollmentResponse>> ListEnrollments(
        Guid id,
        [FromQuery] EnrollmentQuery query,
        CancellationToken cancellationToken)
    {
        var student = await studentService.GetByIdAsync(id, cancellationToken);
        EnforceAdminOrOwner(student.UserId);
        return await enrollmentService.ListByStudentAsync(id, query, cancellationToken);
    }

    /// <summary>
    /// Updates a student profile.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Roles allowed: Admin or resource owner.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<StudentResponse> Update(Guid id, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var current = await studentService.GetByIdAsync(id, cancellationToken);
        EnforceAdminOrOwner(current.UserId);
        return await studentService.UpdateAsync(id, request, cancellationToken);
    }

    /// <summary>
    /// Soft-deletes a student profile.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Role allowed: Admin.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await studentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private void EnforceAdminOrOwner(string resourceUserId)
    {
        if (currentUserService.IsInRole(RoleNames.Admin))
        {
            return;
        }

        var currentUserId = EnsureAuthenticatedUserId();
        if (!string.Equals(currentUserId, resourceUserId, StringComparison.Ordinal))
        {
            throw new ForbiddenException("You can only access your own student profile.");
        }
    }

    private string EnsureAuthenticatedUserId() =>
        currentUserService.UserId ?? throw new UnauthorizedException("Authenticated user id not found.");
}

using aluraProject.Application.Abstractions;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Enrollments;
using aluraProject.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aluraProject.Api.Controllers;

/// <summary>
/// Enrollment endpoints.
/// </summary>
[ApiController]
[Route("enrollments")]
[Route("api/enrollments")]
[Authorize(Roles = RoleNames.Student + "," + RoleNames.Admin)]
public sealed class EnrollmentsController(
    IEnrollmentService enrollmentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Enrolls a student into a course.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Roles allowed: Student, Admin.
    /// Student role: studentId is ignored and the authenticated user profile is used.
    /// Admin role: studentId must be provided in payload.
    /// Duplicate enrollment returns 409.
    /// Business rule violations return 422.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType<EnrollmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user id not found.");
        }

        var enrollment = await enrollmentService.EnrollAsync(
            userId,
            currentUserService.IsInRole(RoleNames.Admin),
            request,
            cancellationToken);

        return Created($"/students/{enrollment.StudentId}/enrollments/{enrollment.Id}", enrollment);
    }
}

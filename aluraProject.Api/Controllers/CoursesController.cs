using System.Security.Claims;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Courses;
using aluraProject.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aluraProject.Api.Controllers;

/// <summary>
/// Course catalog endpoints.
/// </summary>
[ApiController]
[Route("courses")]
[Route("api/courses")]
public sealed class CoursesController(ICourseService courseService) : ControllerBase
{
    /// <summary>
    /// Lists courses with pagination and optional filters.
    /// </summary>
    /// <remarks>
    /// Public endpoint. Pagination: page starts at 1 and pageSize is limited to 100.
    /// Filters: search, category, sortBy, sortOrder.
    /// </remarks>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType<PagedResult<CourseResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<PagedResult<CourseResponse>> List([FromQuery] CourseQuery query, CancellationToken cancellationToken) =>
        courseService.ListAsync(query, cancellationToken);

    /// <summary>
    /// Gets a course by identifier.
    /// </summary>
    /// <remarks>
    /// Public endpoint.
    /// </remarks>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
    [ProducesResponseType<CourseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<CourseResponse> GetById(Guid id, CancellationToken cancellationToken) =>
        courseService.GetByIdAsync(id, cancellationToken);

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Roles allowed: Admin, Instructor.
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Instructor)]
    [ProducesResponseType<CourseResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user id not found.");
        }

        var created = await courseService.CreateAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates a course.
    /// </summary>
    /// <remarks>
    /// Requires JWT. Roles allowed: Admin, Instructor.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Instructor)]
    [ProducesResponseType<CourseResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<CourseResponse> Update(Guid id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken) =>
        courseService.UpdateAsync(id, request, cancellationToken);

    /// <summary>
    /// Soft-deletes a course.
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
        await courseService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

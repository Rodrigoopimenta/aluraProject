using System.Security.Claims;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Courses;
using aluraProject.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aluraProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<PagedResult<CourseResponse>>(StatusCodes.Status200OK)]
    public Task<PagedResult<CourseResponse>> List([FromQuery] CourseQuery query, CancellationToken cancellationToken) =>
        courseService.ListAsync(query, cancellationToken);

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType<CourseResponse>(StatusCodes.Status200OK)]
    public Task<CourseResponse> GetById(Guid id, CancellationToken cancellationToken) =>
        courseService.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Instructor)]
    [ProducesResponseType<CourseResponse>(StatusCodes.Status201Created)]
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Instructor)]
    [ProducesResponseType<CourseResponse>(StatusCodes.Status200OK)]
    public Task<CourseResponse> Update(Guid id, [FromBody] UpdateCourseRequest request, CancellationToken cancellationToken) =>
        courseService.UpdateAsync(id, request, cancellationToken);

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await courseService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

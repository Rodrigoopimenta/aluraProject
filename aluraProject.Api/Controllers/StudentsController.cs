using aluraProject.Application.Abstractions;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Students;
using aluraProject.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aluraProject.Api.Controllers;

[ApiController]
[Route("students")]
[Route("api/students")]
[Authorize]
public sealed class StudentsController(
    IStudentService studentService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var created = await studentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType<PagedResult<StudentResponse>>(StatusCodes.Status200OK)]
    public Task<PagedResult<StudentResponse>> List([FromQuery] StudentQuery query, CancellationToken cancellationToken) =>
        studentService.ListAsync(query, cancellationToken);

    [HttpGet("me")]
    [HttpGet("/me")]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    public Task<StudentResponse> GetMe(CancellationToken cancellationToken)
    {
        var userId = EnsureAuthenticatedUserId();
        return studentService.GetByUserIdAsync(userId, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    public async Task<StudentResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        var student = await studentService.GetByIdAsync(id, cancellationToken);
        EnforceAdminOrOwner(student.UserId);
        return student;
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<StudentResponse>(StatusCodes.Status200OK)]
    public async Task<StudentResponse> Update(Guid id, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var current = await studentService.GetByIdAsync(id, cancellationToken);
        EnforceAdminOrOwner(current.UserId);
        return await studentService.UpdateAsync(id, request, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

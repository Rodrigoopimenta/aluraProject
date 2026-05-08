using System.Security.Claims;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Application.Enrollments;
using aluraProject.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aluraProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.Student + "," + RoleNames.Admin)]
public sealed class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = RoleNames.Student)]
    [ProducesResponseType<EnrollmentResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user id not found.");
        }

        var enrollment = await enrollmentService.EnrollAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(ListMineOrAll), new { }, enrollment);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<EnrollmentResponse>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<EnrollmentResponse>> ListMineOrAll(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("Authenticated user id not found.");
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        return enrollmentService.ListByUserAsync(userId, isAdmin, cancellationToken);
    }
}

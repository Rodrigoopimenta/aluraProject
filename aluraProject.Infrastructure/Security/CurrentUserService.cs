using System.Security.Claims;
using aluraProject.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace aluraProject.Infrastructure.Security;

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public string? UserId =>
        accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? accessor.HttpContext?.User.FindFirstValue("sub");

    public bool IsInRole(string role) => accessor.HttpContext?.User.IsInRole(role) == true;
}

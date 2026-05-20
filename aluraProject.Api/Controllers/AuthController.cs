using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aluraProject.Api.Controllers;

/// <summary>
/// Authentication endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new user account and returns a JWT access token.
    /// </summary>
    /// <remarks>
    /// Public endpoint (no authentication required). Supported roles: Admin, Instructor, Student.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<AuthResponse> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken) =>
        authService.RegisterAsync(request, cancellationToken);

    /// <summary>
    /// Authenticates an existing user and returns a JWT access token.
    /// </summary>
    /// <remarks>
    /// Public endpoint (no authentication required).
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public Task<AuthResponse> Login([FromBody] LoginRequest request, CancellationToken cancellationToken) =>
        authService.LoginAsync(request, cancellationToken);
}

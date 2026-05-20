using aluraProject.Application.Abstractions;
using aluraProject.Application.Abstractions.Services;
using aluraProject.Application.Auth;
using aluraProject.Application.Common.Exceptions;
using aluraProject.Domain.Common;

namespace aluraProject.Application.Services;

public sealed class AuthService(IIdentityService identityService, IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!RoleNames.All.Contains(request.Role))
        {
            throw new ValidationException("Invalid role.");
        }

        var registerResult = await identityService.RegisterAsync(request.Email, request.Password, request.Role, cancellationToken);
        if (!registerResult.Succeeded || string.IsNullOrWhiteSpace(registerResult.UserId))
        {
            throw new ValidationException(string.Join("; ", registerResult.Errors));
        }

        var tokenResult = jwtTokenService.GenerateToken(registerResult.UserId, request.Email, [request.Role]);
        return new AuthResponse(tokenResult.Token, tokenResult.ExpiresAtUtc, request.Email, request.Role);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var loginResult = await identityService.LoginAsync(request.Email, request.Password, cancellationToken);
        if (!loginResult.Succeeded || string.IsNullOrWhiteSpace(loginResult.UserId) || string.IsNullOrWhiteSpace(loginResult.Email))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        var roles = loginResult.Roles.Count == 0 ? new[] { RoleNames.Student } : loginResult.Roles;
        var tokenResult = jwtTokenService.GenerateToken(loginResult.UserId, loginResult.Email, roles);

        return new AuthResponse(tokenResult.Token, tokenResult.ExpiresAtUtc, loginResult.Email, roles.First());
    }
}

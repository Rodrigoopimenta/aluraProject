namespace aluraProject.Application.Auth;

public sealed record RegisterRequest(string Email, string Password, string Role);
public sealed record LoginRequest(string Email, string Password);
public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, string Email, string Role);

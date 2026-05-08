namespace aluraProject.Application.Abstractions;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(string userId, string email, IEnumerable<string> roles);
}

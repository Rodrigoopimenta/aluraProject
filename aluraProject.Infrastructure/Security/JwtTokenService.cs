using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using aluraProject.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace aluraProject.Infrastructure.Security;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public (string Token, DateTime ExpiresAtUtc) GenerateToken(string userId, string email, IEnumerable<string> roles)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "aluraProject";
        var audience = configuration["Jwt:Audience"] ?? "aluraProject.Client";
        var key = configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key) || key.Length < 16)
        {
            throw new InvalidOperationException("Jwt:Key is missing or too short. Configure a secure key via environment variables or user-secrets.");
        }

        var accessTokenMinutes = configuration.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 120;
        if (accessTokenMinutes <= 0)
        {
            accessTokenMinutes = 120;
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(accessTokenMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

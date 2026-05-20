namespace aluraProject.Application.Abstractions;

public interface IIdentityService
{
    Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> RegisterAsync(string email, string password, string role, CancellationToken cancellationToken);
    Task<(bool Succeeded, string? UserId, string? Email, IReadOnlyList<string> Roles)> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken);
}

using aluraProject.Application.Abstractions;
using aluraProject.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace aluraProject.Infrastructure.Security;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    SignInManager<ApplicationUser> signInManager) : IIdentityService
{
    public async Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> RegisterAsync(
        string email,
        string password,
        string role,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            return (false, null, ["Email already registered."]);
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            return (false, null, [$"Role '{role}' does not exist."]);
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return (false, null, createResult.Errors.Select(e => e.Description));
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, role);
        if (!addRoleResult.Succeeded)
        {
            return (false, null, addRoleResult.Errors.Select(e => e.Description));
        }

        return (true, user.Id, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, string? UserId, string? Email, IReadOnlyList<string> Roles)> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (user is null)
        {
            return (false, null, null, Array.Empty<string>());
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            return (false, null, null, Array.Empty<string>());
        }

        var roles = await userManager.GetRolesAsync(user);
        return (true, user.Id, user.Email, roles.ToList());
    }

    public Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken) =>
        userManager.Users.AnyAsync(x => x.Id == userId, cancellationToken);
}

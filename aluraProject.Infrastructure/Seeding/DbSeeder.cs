using aluraProject.Domain.Common;
using aluraProject.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace aluraProject.Infrastructure.Seeding;

public static class DbSeeder
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var dbContext = scopedServices.GetRequiredService<Persistence.AppDbContext>();
        await dbContext.Database.MigrateAsync();

        var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var configuration = scopedServices.GetRequiredService<IConfiguration>();
        string? adminEmail = configuration["Seed:Admin:Email"];
        string? adminPassword = configuration["Seed:Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
        string normalizedAdminEmail = adminEmail.Trim().ToLowerInvariant();

        var admin = await userManager.Users.FirstOrDefaultAsync(x => x.Email == normalizedAdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = normalizedAdminEmail,
                Email = normalizedAdminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Admin seed failed: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
        {
            await userManager.AddToRoleAsync(admin, RoleNames.Admin);
        }
    }
}

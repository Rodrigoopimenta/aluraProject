using aluraProject.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace aluraProject.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public Student? Student { get; set; }
}

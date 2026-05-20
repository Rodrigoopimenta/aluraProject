using System.Net.Mail;

namespace aluraProject.Domain.Entities;

public sealed class Student
{
    private Student() { }

    public Student(string userId, string fullName, string email)
    {
        Id = Guid.NewGuid();
        UserId = NormalizeRequired(userId, "UserId is required.");
        SetFullName(fullName);
        SetEmail(email);
        RegisteredAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
        IsDeleted = false;
    }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime RegisteredAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    public void Update(string fullName, string email)
    {
        SetFullName(fullName);
        SetEmail(email);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private void SetFullName(string fullName)
    {
        var normalized = NormalizeRequired(fullName, "Full name is required.");
        if (normalized.Length < 3 || normalized.Length > 200)
        {
            throw new ArgumentException("Full name must have between 3 and 200 characters.");
        }

        FullName = normalized;
    }

    private void SetEmail(string email)
    {
        var normalized = NormalizeRequired(email, "Email is required.").ToLowerInvariant();
        _ = new MailAddress(normalized);
        Email = normalized;
    }

    private static string NormalizeRequired(string value, string error)
    {
        var normalized = (value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException(error);
        }

        return normalized;
    }
}

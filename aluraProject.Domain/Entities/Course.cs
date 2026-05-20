namespace aluraProject.Domain.Entities;

public sealed class Course
{
    private Course() { }

    public Course(string title, string? description, string category, int workloadHours, string createdByUserId)
    {
        Id = Guid.NewGuid();
        SetTitle(title);
        SetCategory(category);
        SetWorkloadHours(workloadHours);
        Description = NormalizeOptional(description, 1000);
        CreatedByUserId = createdByUserId;
        IsDeleted = false;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public int WorkloadHours { get; private set; }
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    public void Update(string title, string? description, string category, int workloadHours)
    {
        SetTitle(title);
        SetCategory(category);
        SetWorkloadHours(workloadHours);
        Description = NormalizeOptional(description, 1000);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private void SetTitle(string title)
    {
        var normalized = NormalizeRequired(title, 3, 200, "Course title must have between 3 and 200 characters.");
        Title = normalized;
    }

    private void SetCategory(string category)
    {
        var normalized = NormalizeRequired(category, 2, 80, "Category must have between 2 and 80 characters.");
        Category = normalized;
    }

    private void SetWorkloadHours(int workloadHours)
    {
        if (workloadHours <= 0 || workloadHours > 2000)
        {
            throw new ArgumentException("Workload hours must be between 1 and 2000.");
        }

        WorkloadHours = workloadHours;
    }

    private static string NormalizeRequired(string value, int min, int max, string error)
    {
        var normalized = (value ?? string.Empty).Trim();
        if (normalized.Length < min || normalized.Length > max)
        {
            throw new ArgumentException(error);
        }

        return normalized;
    }

    private static string? NormalizeOptional(string? value, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > max)
        {
            throw new ArgumentException($"Description must have at most {max} characters.");
        }

        return normalized;
    }
}

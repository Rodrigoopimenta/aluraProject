namespace aluraProject.Domain.Entities;

public sealed class Enrollment
{
    private Enrollment() { }

    public Enrollment(Guid studentId, Guid courseId)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        CourseId = courseId;
        Status = EnrollmentStatus.Active;
        EnrolledAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrolledAtUtc { get; private set; }

    public Student? Student { get; private set; }
    public Course? Course { get; private set; }

    public void Cancel()
    {
        Status = EnrollmentStatus.Cancelled;
    }
}

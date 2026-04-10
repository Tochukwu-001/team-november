namespace LMS.Domain.Entities;
public class Enrollment : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!; 

    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = default!; 

    public DateTime EnrolledAt { get; private set; }

    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public new bool IsActive => !IsCompleted;

    private Enrollment() { } // EF Core

    // Factory
    public static Enrollment Create(Guid userId, Guid courseId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (courseId == Guid.Empty)
            throw new ArgumentException("Course is required");

        return new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };
    }

    // Domain behavior
    public void MarkAsCompleted()
    {
        if (IsCompleted)
            throw new InvalidOperationException("Course already completed");

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;

        SetUpdated();
    }
}

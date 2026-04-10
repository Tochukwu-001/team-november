namespace LMS.Domain.Entities;
public class LessonProgress : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }

    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;
    public Lesson Lesson { get; private set; } = default!;

    private LessonProgress() { } // EF Core
    // Factory
    public static LessonProgress Create(Guid userId, Guid lessonId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User is required");
        }  

        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("Lesson is required");
        } 

        return new LessonProgress
        {
            UserId = userId,
            LessonId = lessonId,
            IsCompleted = false
        };
    }

    // Domain behavior
    public void MarkAsCompleted()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Lesson already completed");
        } 

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;

        SetUpdated();
    }
}
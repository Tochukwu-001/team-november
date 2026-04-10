namespace LMS.Domain.Entities;
public class Progress : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }

    public int TotalLessons { get; private set; }
    public int CompletedLessons { get; private set; }

    public double Percentage { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;
    public Course Course { get; private set; } = default!;

    private Progress() { }

    // Factory
    public static Progress Create(Guid userId, Guid courseId, int totalLessons)
    {
        if (userId == Guid.Empty)
        {

            throw new ArgumentException("User is required");
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("Course is required");
        } 

        if (totalLessons < 0)
        {
            throw new ArgumentException("Invalid lesson count");
        }  

        return new Progress
        {
            UserId = userId,
            CourseId = courseId,
            TotalLessons = totalLessons,
            CompletedLessons = 0,
            Percentage = 0
        };
    }

    // Domain behavior
    public void UpdateProgress(int completedLessons)
    {
        if (completedLessons < 0 || completedLessons > TotalLessons)
        {
            throw new ArgumentException("Invalid completed lessons");
        }

        CompletedLessons = completedLessons;

        Percentage = TotalLessons == 0
            ? 0
            : (double)CompletedLessons / TotalLessons * 100;

        SetUpdated();
    }
}

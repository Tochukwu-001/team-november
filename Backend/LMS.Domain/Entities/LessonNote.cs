namespace LMS.Domain.Entities;

public class LessonNote : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }
    public string Content { get; private set; } = default!;

    public User User { get; private set; } = default!;
    public Lesson Lesson { get; private set; } = default!;

    private LessonNote() { }

    public static LessonNote Create(Guid userId, Guid lessonId, string content)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (lessonId == Guid.Empty)
            throw new ArgumentException("Lesson is required");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Note content is required");

        return new LessonNote
        {
            UserId = userId,
            LessonId = lessonId,
            Content = content.Trim()
        };
    }

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Note content is required");

        Content = content.Trim();
        SetUpdated();
    }
}

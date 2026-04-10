namespace LMS.Domain.Entities;
public class Lesson : BaseEntity
{
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = default!; // 🔥 navigation

    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public int? EstimatedMinutes { get; private set; }
    public int Order { get; private set; }

    private readonly List<LessonContent> _contents = new();
    public IReadOnlyCollection<LessonContent> Contents => _contents.AsReadOnly();

    public int ContentCount => _contents.Count;

    private Lesson() { } // For EF Core

    public static Lesson Create(Guid courseId, string title, int order, string? description = null, int? estimatedMinutes = null)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course is required");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Lesson title is required");

        if (order <= 0)
            throw new ArgumentException("Order must be greater than zero");

        if (estimatedMinutes.HasValue && estimatedMinutes.Value <= 0)
            throw new ArgumentException("Estimated minutes must be greater than zero");

        return new Lesson
        {
            CourseId = courseId,
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            EstimatedMinutes = estimatedMinutes,
            Order = order
        };
    }

    public void UpdateDetails(string title, string? description = null, int? estimatedMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Lesson title is required");

        if (estimatedMinutes.HasValue && estimatedMinutes.Value <= 0)
            throw new ArgumentException("Estimated minutes must be greater than zero");

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        EstimatedMinutes = estimatedMinutes;
        SetUpdated();
    }

    public void AddContent(LessonContent content)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }


        if (content.LessonId != Id)
        {
            throw new InvalidOperationException("Content does not belong to this lesson");
        }
        _contents.Add(content);
    }
}

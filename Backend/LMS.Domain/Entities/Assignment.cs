namespace LMS.Domain.Entities;

public class Assignment : BaseEntity
{
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public DateTime DueDate { get; private set; }

    private readonly List<Submission> _submissions = new();
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();

    private Assignment() { }

    // Factory
    public static Assignment Create(Guid courseId, string title, string description, DateTime dueDate)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course is required");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Assignment title is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Assignment description is required");

        if (dueDate <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future");

        return new Assignment
        {
            CourseId = courseId,
            Title = title.Trim(),
            Description = description.Trim(),
            DueDate = dueDate
        };
    }

    public void Update(string title, string description, DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required");

        if (dueDate <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future");

        Title = title.Trim();
        Description = description.Trim();
        DueDate = dueDate;
    }

    public void AddSubmission(Submission submission)
    {
        if (submission == null)
            throw new ArgumentNullException(nameof(submission));

        _submissions.Add(submission);
    }

    public bool IsPastDue() => DateTime.UtcNow > DueDate;
}

namespace LMS.Domain.Entities;

public class Submission : BaseEntity
{
    public Guid AssignmentId { get; private set; }
    public Assignment Assignment { get; private set; } = default!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public string? Answer { get; private set; }
    public string? AttachmentUrl { get; private set; }
    public string? AttachmentName { get; private set; }
    public string? AttachmentContentType { get; private set; }
    public long? AttachmentSizeBytes { get; private set; }
    public string? Feedback { get; private set; }
    public decimal? Score { get; private set; }
    public DateTime SubmittedAt { get; private set; }

    private Submission() { }

    // Factory
    public static Submission Create(Guid assignmentId, Guid userId, string? answer)
    {
        if (assignmentId == Guid.Empty)
            throw new ArgumentException("Assignment is required");

        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (!string.IsNullOrWhiteSpace(answer) && answer.Length > 5000)
            throw new ArgumentException("Answer is too long");
        return new Submission
        {
            AssignmentId = assignmentId,
            UserId = userId,
            Answer = string.IsNullOrWhiteSpace(answer) ? null : answer.Trim(),
            SubmittedAt = DateTime.UtcNow
        };
    }

    public void AttachFile(string attachmentUrl, string attachmentName, string? attachmentContentType, long? attachmentSizeBytes)
    {
        if (string.IsNullOrWhiteSpace(attachmentUrl))
            throw new ArgumentException("Attachment URL is required");

        if (string.IsNullOrWhiteSpace(attachmentName))
            throw new ArgumentException("Attachment name is required");

        AttachmentUrl = attachmentUrl.Trim();
        AttachmentName = attachmentName.Trim();
        AttachmentContentType = string.IsNullOrWhiteSpace(attachmentContentType) ? null : attachmentContentType.Trim();
        AttachmentSizeBytes = attachmentSizeBytes > 0 ? attachmentSizeBytes : null;
        SetUpdated();
    }

    // Domain behavior
    public void Grade(decimal score, string? feedback = null)
    {
        if (Score.HasValue)
            throw new InvalidOperationException("Submission already graded");

        if (score < 0 || score > 100)
            throw new ArgumentException("Score must be between 0 and 100");

        Score = score;
        Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
        SetUpdated();
    }
}

using LMS.Domain.Enums;

namespace LMS.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public string? ActionUrl { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? actionUrl = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Notification title is required");

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Notification message is required");

        if (!Enum.IsDefined(typeof(NotificationType), type))
            throw new ArgumentException("Invalid notification type");

        return new Notification
        {
            UserId = userId,
            Type = type,
            Title = title.Trim(),
            Message = message.Trim(),
            ActionUrl = string.IsNullOrWhiteSpace(actionUrl) ? null : actionUrl.Trim()
        };
    }

    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
        SetUpdated();
    }
}

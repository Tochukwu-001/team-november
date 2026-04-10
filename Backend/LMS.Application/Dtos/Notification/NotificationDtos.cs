using LMS.Domain.Enums;

namespace LMS.Application.DTOs.Notification;

public record NotificationResponse(
    Guid Id,
    NotificationType Type,
    string Title,
    string Message,
    string? ActionUrl,
    bool IsRead,
    DateTime CreatedAt,
    DateTime? ReadAt
);

public record CreateNotificationRequest(
    Guid UserId,
    NotificationType Type,
    string Title,
    string Message,
    string? ActionUrl = null,
    bool SendEmail = false
);

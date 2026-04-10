using LMS.Application.Common;
using LMS.Application.DTOs.Notification;

namespace LMS.Application.Interfaces.Services;

public interface INotificationService
{
    Task NotifyUserAsync(CreateNotificationRequest request);
    Task NotifyUsersAsync(IEnumerable<CreateNotificationRequest> requests);
    Task<BaseResponse<IEnumerable<NotificationResponse>>> GetMyNotificationsAsync();
    Task<BaseResponse<int>> GetUnreadCountAsync();
    Task<BaseResponse<bool>> MarkAsReadAsync(Guid notificationId);
    Task<BaseResponse<bool>> MarkAllAsReadAsync();
}

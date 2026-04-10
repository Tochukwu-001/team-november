using LMS.Application.Common;
using LMS.Application.DTOs.Notification;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task NotifyUserAsync(CreateNotificationRequest request)
    {
        var notification = Notification.Create(
            request.UserId,
            request.Type,
            request.Title,
            request.Message,
            request.ActionUrl
        );

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        if (!request.SendEmail)
            return;

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return;

        await _emailService.SendNotificationEmailAsync(
            user.Email,
            user.FirstName,
            request.Title,
            request.Message,
            request.ActionUrl
        );
    }

    public async Task NotifyUsersAsync(IEnumerable<CreateNotificationRequest> requests)
    {
        var requestList = requests.ToList();
        if (requestList.Count == 0)
            return;

        var emailQueue = new List<(string Email, string FirstName, string Title, string Message, string? ActionUrl)>();

        foreach (var request in requestList)
        {
            var notification = Notification.Create(
                request.UserId,
                request.Type,
                request.Title,
                request.Message,
                request.ActionUrl
            );

            await _notificationRepository.AddAsync(notification);

            if (!request.SendEmail)
                continue;

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                continue;

            emailQueue.Add((user.Email, user.FirstName, request.Title, request.Message, request.ActionUrl));
        }

        await _unitOfWork.SaveChangesAsync();

        foreach (var email in emailQueue)
        {
            await _emailService.SendNotificationEmailAsync(
                email.Email,
                email.FirstName,
                email.Title,
                email.Message,
                email.ActionUrl
            );
        }
    }

    public async Task<BaseResponse<IEnumerable<NotificationResponse>>> GetMyNotificationsAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<IEnumerable<NotificationResponse>>.Fail("Unauthorized");

        var notifications = await _notificationRepository.GetByUserIdAsync(_currentUserService.UserId.Value);
        return BaseResponse<IEnumerable<NotificationResponse>>.Ok(notifications.Select(MapToResponse));
    }

    public async Task<BaseResponse<int>> GetUnreadCountAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<int>.Fail("Unauthorized");

        var unreadCount = await _notificationRepository.GetUnreadCountAsync(_currentUserService.UserId.Value);
        return BaseResponse<int>.Ok(unreadCount);
    }

    public async Task<BaseResponse<bool>> MarkAsReadAsync(Guid notificationId)
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<bool>.Fail("Unauthorized");

        var notification = await _notificationRepository.GetByIdForUserAsync(notificationId, _currentUserService.UserId.Value);
        if (notification == null)
            return BaseResponse<bool>.Fail("Notification not found");

        notification.MarkAsRead();
        await _notificationRepository.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<bool>.Ok(true, "Notification marked as read");
    }

    public async Task<BaseResponse<bool>> MarkAllAsReadAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<bool>.Fail("Unauthorized");

        var notifications = await _notificationRepository.GetByUserIdAsync(_currentUserService.UserId.Value);

        foreach (var notification in notifications.Where(item => !item.IsRead))
        {
            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification);
        }

        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<bool>.Ok(true, "All notifications marked as read");
    }

    private static NotificationResponse MapToResponse(Notification notification)
    {
        return new NotificationResponse(
            notification.Id,
            notification.Type,
            notification.Title,
            notification.Message,
            notification.ActionUrl,
            notification.IsRead,
            notification.CreatedAt,
            notification.ReadAt
        );
    }
}

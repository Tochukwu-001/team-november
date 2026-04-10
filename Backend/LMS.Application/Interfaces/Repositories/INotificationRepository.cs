using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetByUserIdAsync(Guid userId);
    Task<Notification?> GetByIdForUserAsync(Guid notificationId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
}

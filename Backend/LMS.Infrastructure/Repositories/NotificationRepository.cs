using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<List<Notification>> GetAllAsync()
    {
        return await _context.Notifications
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification?> GetByIdForUserAsync(Guid notificationId, Guid userId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(item => item.Id == notificationId && item.UserId == userId);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(item => item.UserId == userId && !item.IsRead);
    }

    public async Task AddAsync(Notification entity)
    {
        await _context.Notifications.AddAsync(entity);
    }

    public Task UpdateAsync(Notification entity)
    {
        _context.Notifications.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Notification entity)
    {
        _context.Notifications.Remove(entity);
        return Task.CompletedTask;
    }
}

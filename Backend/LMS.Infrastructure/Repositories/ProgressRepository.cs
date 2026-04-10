using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class ProgressRepository : IProgressRepository
{
    private readonly ApplicationDbContext _context;

    public ProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Progress?> GetByIdAsync(Guid id)
    {
        return await _context.Progresses.FindAsync(id);
    }

    public async Task<List<Progress>> GetAllAsync()
    {
        return await _context.Progresses.ToListAsync();
    }

    public async Task<Progress?> GetByUserAndCourseAsync(Guid userId, Guid courseId)
    {
        return await _context.Progresses
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);
    }

    public async Task<List<Progress>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Progresses
            .AsNoTracking()
            .Include(p => p.Course)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.Percentage)
            .ToListAsync();
    }

    public async Task AddAsync(Progress entity)
    {
        await _context.Progresses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Progress entity)
    {
        _context.Progresses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Progress entity)
    {
        _context.Progresses.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

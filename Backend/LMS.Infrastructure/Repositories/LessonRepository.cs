using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons
            .Include(l => l.Contents)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<List<Lesson>> GetAllAsync()
    {
        return await _context.Lessons
            .Include(l => l.Contents)
            .ToListAsync();
    }

    public async Task<List<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Lessons
            .Include(l => l.Contents)
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task AddAsync(Lesson entity)
    {
        await _context.Lessons.AddAsync(entity);

    }

    public Task UpdateAsync(Lesson entity)
    {
        _context.Lessons.Update(entity);
        return Task.CompletedTask;

    }

    public Task DeleteAsync(Lesson entity)
    {
        _context.Lessons.Remove(entity);
        return Task.CompletedTask;

    }
}

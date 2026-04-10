using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class LessonProgressRepository : ILessonProgressRepository
{
    private readonly ApplicationDbContext _context;

    public LessonProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LessonProgress?> GetByIdAsync(Guid id)
    {
        return await _context.LessonProgresses.FindAsync(id);
    }

    public async Task<List<LessonProgress>> GetAllAsync()
    {
        return await _context.LessonProgresses.ToListAsync();
    }

    public async Task<LessonProgress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
    {
        return await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId);
    }

    public async Task<int> GetCompletedLessonsCountAsync(Guid userId, Guid courseId)
    {
        return await _context.LessonProgresses
            .CountAsync(lp => lp.UserId == userId && lp.Lesson.CourseId == courseId && lp.IsCompleted);
    }

    public async Task AddAsync(LessonProgress entity)
    {
        await _context.LessonProgresses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LessonProgress entity)
    {
        _context.LessonProgresses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(LessonProgress entity)
    {
        _context.LessonProgresses.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

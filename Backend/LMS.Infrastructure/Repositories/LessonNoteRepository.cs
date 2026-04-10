using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class LessonNoteRepository : ILessonNoteRepository
{
    private readonly ApplicationDbContext _context;

    public LessonNoteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LessonNote?> GetByIdAsync(Guid id)
    {
        return await _context.Set<LessonNote>().FindAsync(id);
    }

    public async Task<List<LessonNote>> GetAllAsync()
    {
        return await _context.Set<LessonNote>().ToListAsync();
    }

    public async Task<LessonNote?> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
    {
        return await _context.Set<LessonNote>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.LessonId == lessonId);
    }

    public async Task AddAsync(LessonNote entity)
    {
        await _context.Set<LessonNote>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LessonNote entity)
    {
        _context.Set<LessonNote>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(LessonNote entity)
    {
        _context.Set<LessonNote>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}

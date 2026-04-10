using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _context.Courses.Include(c => c.Instructor).ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetByInstructorIdAsync(Guid instructorId)
    {
        return await _context.Courses
            .Where(c => c.InstructorId == instructorId)
            .ToListAsync();
    }

    public async Task AddAsync(Course entity)
    {
        await _context.Courses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course entity)
    {
        _context.Courses.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course entity)
    {
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

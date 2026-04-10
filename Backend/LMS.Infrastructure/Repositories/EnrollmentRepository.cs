using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByIdAsync(Guid id)
    {
        return await _context.Enrollments.FindAsync(id);
    }

    public async Task<List<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments.ToListAsync();
    }

    public async Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId)
    {
        return await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public async Task<List<Enrollment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Enrollments
            .Where(item => item.CourseId == courseId)
            .ToListAsync();
    }

    public async Task AddAsync(Enrollment entity)
    {
        await _context.Enrollments.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Enrollment entity)
    {
        _context.Enrollments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Enrollment entity)
    {
        _context.Enrollments.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

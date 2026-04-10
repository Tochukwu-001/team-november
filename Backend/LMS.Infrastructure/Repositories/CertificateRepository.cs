using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private readonly ApplicationDbContext _context;

    public CertificateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Certificate?> GetByIdAsync(Guid id)
    {
        return await _context.Certificates
            .Include(c => c.User)
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Certificate>> GetAllAsync()
    {
        return await _context.Certificates
            .Include(c => c.User)
            .Include(c => c.Course)
            .ToListAsync();
    }

    public async Task<List<Certificate>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Certificates
            .Include(c => c.Course)
            .Include(c => c.User)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Certificate?> GetByUserAndCourseAsync(Guid userId, Guid courseId)
    {
        return await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);
    }

    public async Task AddAsync(Certificate entity)
    {
        await _context.Certificates.AddAsync(entity);
    }

    public Task UpdateAsync(Certificate entity)
    {
        _context.Certificates.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Certificate entity)
    {
        _context.Certificates.Remove(entity);
        return Task.CompletedTask;
    }
}

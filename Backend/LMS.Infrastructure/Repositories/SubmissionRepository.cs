using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetByIdAsync(Guid id)
    {
        return await _context.Submissions
            .AsNoTracking()
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Submission>> GetAllAsync()
    {
        return await _context.Submissions
            .AsNoTracking()
            .Include(s => s.Assignment)
            .Include(s => s.User)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<Submission>> GetByAssignmentIdAsync(Guid assignmentId)
    {
        return await _context.Submissions
            .AsNoTracking()
            .Where(s => s.AssignmentId == assignmentId)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
            .Include(s => s.User)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<Submission>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Submissions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Submission?> GetByAssignmentAndUserAsync(Guid assignmentId, Guid userId)
    {
        return await _context.Submissions
            .AsNoTracking()
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.UserId == userId);
    }

    public async Task AddAsync(Submission entity)
    {
        await _context.Submissions.AddAsync(entity);
    }

    public Task UpdateAsync(Submission entity)
    {
        _context.Submissions.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Submission entity)
    {
        _context.Submissions.Remove(entity);
        return Task.CompletedTask;
    }
}

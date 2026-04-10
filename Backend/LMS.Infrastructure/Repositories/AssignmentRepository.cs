using LMS.Application.Common;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id)
    {
        return await _context.Assignments
            .AsNoTracking()
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Assignment>> GetAllAsync()
    {
        return await _context.Assignments
            .AsNoTracking()
            .OrderBy(a => a.DueDate)
            .Include(a => a.Course)
            .ToListAsync();
    }

    public async Task<List<Assignment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Assignments
            .AsNoTracking()
            .Where(a => a.CourseId == courseId)
            .OrderBy(a => a.DueDate)
            .Include(a => a.Course)
            .ToListAsync();
    }

    public async Task AddAsync(Assignment entity)
    {
        await _context.Assignments.AddAsync(entity);
    }

    public Task UpdateAsync(Assignment entity)
    {
        _context.Assignments.Update(entity);
        return Task.CompletedTask;

    }

    public Task DeleteAsync(Assignment entity)
    {
        _context.Assignments.Remove(entity);
        return Task.CompletedTask;

    }

    public async Task<PagedResult<Assignment>> GetPagedAsync(int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var query = _context.Assignments
            .AsNoTracking()
            .OrderBy(a => a.DueDate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(a => a.Course)
            .ToListAsync();

        return new PagedResult<Assignment>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<Assignment>> GetByCoursePagedAsync(Guid courseId, int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var query = _context.Assignments
            .AsNoTracking()
            .Where(a => a.CourseId == courseId)
            .OrderBy(a => a.DueDate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(a => a.Course)
            .ToListAsync();

        return new PagedResult<Assignment>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}

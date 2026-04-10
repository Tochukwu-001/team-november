using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class InstructorRoleRequestRepository : IInstructorRoleRequestRepository
{
    private readonly ApplicationDbContext _context;

    public InstructorRoleRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InstructorRoleRequest?> GetByIdAsync(Guid id)
    {
        return await _context.Set<InstructorRoleRequest>()
            .Include(x => x.User)
            .Include(x => x.ReviewedByUser)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<InstructorRoleRequest>> GetAllAsync()
    {
        return await _context.Set<InstructorRoleRequest>()
            .Include(x => x.User)
            .Include(x => x.ReviewedByUser)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<InstructorRoleRequest>> GetAllWithUsersAsync(RoleRequestStatus? status = null)
    {
        var query = _context.Set<InstructorRoleRequest>()
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.ReviewedByUser)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<InstructorRoleRequest?> GetPendingByUserIdAsync(Guid userId)
    {
        return await _context.Set<InstructorRoleRequest>()
            .Include(x => x.User)
            .Include(x => x.ReviewedByUser)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == RoleRequestStatus.Pending);
    }

    public async Task AddAsync(InstructorRoleRequest entity)
    {
        await _context.Set<InstructorRoleRequest>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(InstructorRoleRequest entity)
    {
        _context.Set<InstructorRoleRequest>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(InstructorRoleRequest entity)
    {
        _context.Set<InstructorRoleRequest>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}

using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(team => team.Members)
            .FirstOrDefaultAsync(team => team.Id == id);
    }

    public async Task<List<Team>> GetAllAsync()
    {
        return await _context.Teams.ToListAsync();
    }

    public async Task<List<Team>> GetAllWithMembersAsync()
    {
        return await _context.Teams
            .Include(team => team.Members)
            .ToListAsync();
    }

    public async Task<Team?> GetByNameAsync(string name)
    {
        return await _context.Teams
            .FirstOrDefaultAsync(team => team.Name.ToLower() == name.Trim().ToLower());
    }

    public async Task<Team?> GetByMemberIdAsync(Guid userId)
    {
        return await _context.Teams
            .Include(team => team.Members)
            .FirstOrDefaultAsync(team => team.Members.Any(member => member.Id == userId));
    }

    public async Task AddAsync(Team entity)
    {
        await _context.Teams.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Team entity)
    {
        _context.Teams.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Team entity)
    {
        _context.Teams.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(user => user.Team)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(user => user.Team)
            .ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(user => user.Team)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
    }

    public async Task AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

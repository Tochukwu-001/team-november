using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordResetTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken?> GetByIdAsync(Guid id)
    {
        return await _context.PasswordResetTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<PasswordResetToken>> GetAllAsync()
    {
        return await _context.PasswordResetTokens
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(PasswordResetToken entity)
    {
        await _context.PasswordResetTokens.AddAsync(entity);
    }

    public Task UpdateAsync(PasswordResetToken entity)
    {
        _context.PasswordResetTokens.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PasswordResetToken entity)
    {
        _context.PasswordResetTokens.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<PasswordResetToken>> GetValidTokensAsync()
    {
        return await _context.PasswordResetTokens
            .Include(t => t.User)
            .Where(t => !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<int> InvalidateUserTokensAsync(Guid userId)
    {
        var tokens = await _context.PasswordResetTokens
        .Where(t => t.UserId == userId
         && !t.IsUsed
         && t.ExpiresAt > DateTime.UtcNow)
        .ToListAsync();

        foreach (var token in tokens)
        {
            token.MarkAsUsed();
        }

        return tokens.Count;
    }
}

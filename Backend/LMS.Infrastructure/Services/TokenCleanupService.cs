using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LMS.Application.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TokenCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var expiredTokens = await context.PasswordResetTokens
                .Where(x => x.ExpiresAt < DateTime.UtcNow || x.IsUsed)
                .ToListAsync(stoppingToken);

            if (expiredTokens.Any())
            {
                context.PasswordResetTokens.RemoveRange(expiredTokens);
                await context.SaveChangesAsync(stoppingToken);
            }

            // ? Run every 1 hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}

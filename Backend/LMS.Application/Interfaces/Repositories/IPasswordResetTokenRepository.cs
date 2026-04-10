using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    Task<List<PasswordResetToken>> GetValidTokensAsync();
    Task<int> InvalidateUserTokensAsync(Guid userId);
}
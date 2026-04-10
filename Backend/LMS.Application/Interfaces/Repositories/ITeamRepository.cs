using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface ITeamRepository : IRepository<Team>
{
    Task<Team?> GetByNameAsync(string name);
    Task<List<Team>> GetAllWithMembersAsync();
    Task<Team?> GetByMemberIdAsync(Guid userId);
}

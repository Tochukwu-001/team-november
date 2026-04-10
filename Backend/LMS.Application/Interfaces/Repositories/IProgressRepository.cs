using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface IProgressRepository : IRepository<Progress>
{
    Task<Progress?> GetByUserAndCourseAsync(Guid userId, Guid courseId);
    Task<List<Progress>> GetByUserIdAsync(Guid userId);
}

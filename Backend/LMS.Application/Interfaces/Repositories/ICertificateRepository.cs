using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface ICertificateRepository : IRepository<Certificate>
{
    Task<List<Certificate>> GetByUserIdAsync(Guid userId);
    Task<Certificate?> GetByUserAndCourseAsync(Guid userId, Guid courseId);
}

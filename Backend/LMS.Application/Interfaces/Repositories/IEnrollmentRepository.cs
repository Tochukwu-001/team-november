using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId);
    Task<List<Enrollment>> GetByUserIdAsync(Guid userId);
    Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId);
}

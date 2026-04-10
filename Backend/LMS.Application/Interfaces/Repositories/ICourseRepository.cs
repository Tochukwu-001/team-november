using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetByInstructorIdAsync(Guid instructorId);
}

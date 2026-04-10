using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<List<Lesson>> GetByCourseIdAsync(Guid courseId);
}

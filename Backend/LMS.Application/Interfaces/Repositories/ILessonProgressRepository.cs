using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface ILessonProgressRepository : IRepository<LessonProgress>
{
    Task<LessonProgress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId);
    Task<int> GetCompletedLessonsCountAsync(Guid userId, Guid courseId);
}

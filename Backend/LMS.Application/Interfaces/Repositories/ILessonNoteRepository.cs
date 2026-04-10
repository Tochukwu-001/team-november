using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface ILessonNoteRepository : IRepository<LessonNote>
{
    Task<LessonNote?> GetByUserAndLessonAsync(Guid userId, Guid lessonId);
}

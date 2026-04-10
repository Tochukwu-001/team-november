using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<List<Submission>> GetByAssignmentIdAsync(Guid assignmentId);
    Task<List<Submission>> GetByUserIdAsync(Guid userId);
    Task<Submission?> GetByAssignmentAndUserAsync(Guid assignmentId, Guid userId);
}

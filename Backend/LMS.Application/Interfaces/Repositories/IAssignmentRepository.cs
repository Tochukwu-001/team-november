using LMS.Application.Common;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<List<Assignment>> GetByCourseIdAsync(Guid courseId);

    Task<PagedResult<Assignment>> GetPagedAsync(int page, int pageSize);

    Task<PagedResult<Assignment>> GetByCoursePagedAsync(Guid courseId, int page, int pageSize);
}

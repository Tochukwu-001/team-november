using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Interfaces.Repositories;

public interface IInstructorRoleRequestRepository : IRepository<InstructorRoleRequest>
{
    Task<InstructorRoleRequest?> GetPendingByUserIdAsync(Guid userId);
    Task<List<InstructorRoleRequest>> GetAllWithUsersAsync(RoleRequestStatus? status = null);
}

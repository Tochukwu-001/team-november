using LMS.Application.Common;
using LMS.Application.DTOs.Role;

namespace LMS.Application.Interfaces.Services;

public interface IRoleManagementService
{
    Task<BaseResponse<InstructorRoleRequestResponse>> RequestInstructorRoleAsync(RequestInstructorRoleRequest request);
    Task<BaseResponse<IEnumerable<InstructorRoleRequestResponse>>> GetRoleRequestsAsync(string? status);
    Task<BaseResponse<InstructorRoleRequestResponse>> ApproveRoleRequestAsync(Guid roleRequestId);
    Task<BaseResponse<InstructorRoleRequestResponse>> RejectRoleRequestAsync(ReviewRoleRequest request);
    Task<BaseResponse<UserRoleAssignmentResponse>> AssignRoleAsync(AssignUserRoleRequest request);
}

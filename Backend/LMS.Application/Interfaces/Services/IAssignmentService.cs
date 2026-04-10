using LMS.Application.Common;
using LMS.Application.DTOs.Assignment;

namespace LMS.Application.Interfaces.Services;

public interface IAssignmentService
{
    Task<BaseResponse<AssignmentResponse>> CreateAssignmentAsync(CreateAssignmentRequest request);
    Task<BaseResponse<IEnumerable<AssignmentResponse>>> GetAssignmentsByCourseIdAsync(Guid courseId);
    Task<BaseResponse<PagedResult<AssignmentResponse>>> GetPagedAsync(int page, int pageSize);
    Task<BaseResponse<IEnumerable<LearnerAssignmentResponse>>> GetMyAssignmentsAsync(string? status);
}

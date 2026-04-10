using LMS.Application.Common;
using LMS.Application.DTOs.Dashboard;

namespace LMS.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<BaseResponse<DashboardResponse>> GetMyDashboardAsync();
    Task<BaseResponse<AdminDashboardResponse>> GetAdminDashboardAsync();
    Task<BaseResponse<InstructorDashboardResponse>> GetInstructorDashboardAsync();
}

using LMS.Application.Common;
using LMS.Application.DTOs.Dashboard;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;

namespace LMS.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly ICurrentUserService _currentUserService;

    public DashboardService(IDashboardRepository dashboardRepository, ICurrentUserService currentUserService)
    {
        _dashboardRepository = dashboardRepository;
        _currentUserService = currentUserService;
    }

    public async Task<BaseResponse<DashboardResponse>> GetMyDashboardAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<DashboardResponse>.Fail("Unauthorized");

        var dashboard = await _dashboardRepository.GetDashboardAsync(_currentUserService.UserId.Value);
        if (dashboard == null)
            return BaseResponse<DashboardResponse>.Fail("User not found");

        return BaseResponse<DashboardResponse>.Ok(dashboard);
    }

    public async Task<BaseResponse<AdminDashboardResponse>> GetAdminDashboardAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<AdminDashboardResponse>.Fail("Unauthorized");

        var dashboard = await _dashboardRepository.GetAdminDashboardAsync(_currentUserService.UserId.Value);
        if (dashboard == null)
            return BaseResponse<AdminDashboardResponse>.Fail("User not found");

        return BaseResponse<AdminDashboardResponse>.Ok(dashboard);
    }

    public async Task<BaseResponse<InstructorDashboardResponse>> GetInstructorDashboardAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<InstructorDashboardResponse>.Fail("Unauthorized");

        var dashboard = await _dashboardRepository.GetInstructorDashboardAsync(_currentUserService.UserId.Value);
        if (dashboard == null)
            return BaseResponse<InstructorDashboardResponse>.Fail("User not found");

        return BaseResponse<InstructorDashboardResponse>.Ok(dashboard);
    }
}

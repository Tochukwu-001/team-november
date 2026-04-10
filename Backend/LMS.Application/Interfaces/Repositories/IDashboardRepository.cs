using LMS.Application.DTOs.Dashboard;

namespace LMS.Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    Task<DashboardResponse?> GetDashboardAsync(
        Guid userId,
        int continueLearningLimit = 3,
        int deadlineLimit = 3,
        int activityLimit = 5,
        int teamMemberPreviewLimit = 3
    );

    Task<AdminDashboardResponse?> GetAdminDashboardAsync(
        Guid userId,
        int activityLimit = 8
    );

    Task<InstructorDashboardResponse?> GetInstructorDashboardAsync(
        Guid userId,
        int courseLimit = 6,
        int reviewLimit = 8,
        int activityLimit = 8
    );
}

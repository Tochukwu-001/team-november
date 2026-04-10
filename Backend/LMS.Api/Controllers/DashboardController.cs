using LMS.Application.Common;
using LMS.Application.DTOs.Dashboard;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Get the authenticated user's dashboard overview
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(BaseResponse<DashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyDashboard()
    {
        var result = await _dashboardService.GetMyDashboardAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get the admin dashboard overview
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<AdminDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var result = await _dashboardService.GetAdminDashboardAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get the instructor dashboard overview
    /// </summary>
    [HttpGet("instructor")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(BaseResponse<InstructorDashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInstructorDashboard()
    {
        var result = await _dashboardService.GetInstructorDashboardAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

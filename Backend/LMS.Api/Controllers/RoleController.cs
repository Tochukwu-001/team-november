using LMS.Application.Common;
using LMS.Application.DTOs.Role;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class RoleController : ControllerBase
{
    private readonly IRoleManagementService _roleManagementService;

    public RoleController(IRoleManagementService roleManagementService)
    {
        _roleManagementService = roleManagementService;
    }

    [HttpPost("role/request-instructor")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<InstructorRoleRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestInstructorRole([FromBody] RequestInstructorRoleRequest request)
    {
        var result = await _roleManagementService.RequestInstructorRoleAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("admin/role-requests")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<InstructorRoleRequestResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRoleRequests([FromQuery] string? status = null)
    {
        var result = await _roleManagementService.GetRoleRequestsAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("admin/approve-role/{roleRequestId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<InstructorRoleRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveRole(Guid roleRequestId)
    {
        var result = await _roleManagementService.ApproveRoleRequestAsync(roleRequestId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("admin/reject-role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<InstructorRoleRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectRole([FromBody] ReviewRoleRequest request)
    {
        var result = await _roleManagementService.RejectRoleRequestAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("admin/assign-role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<UserRoleAssignmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRole([FromBody] AssignUserRoleRequest request)
    {
        var result = await _roleManagementService.AssignRoleAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

using LMS.Application.Common;
using LMS.Application.DTOs.Team;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    /// <summary>
    /// Get all teams with members
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<TeamResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTeams()
    {
        var result = await _teamService.GetTeamsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get the authenticated user's team
    /// </summary>
    [HttpGet("my-team")]
    [ProducesResponseType(typeof(BaseResponse<TeamResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyTeam()
    {
        var result = await _teamService.GetMyTeamAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

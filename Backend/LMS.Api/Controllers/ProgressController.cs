using LMS.Application.Common;
using LMS.Application.DTOs.Progress;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<ProgressOverviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMyProgress()
    {
        var result = await _progressService.GetMyProgressAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

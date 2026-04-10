using LMS.Application.Common;
using LMS.Application.DTOs.Certificate;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CertificateController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificateController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    /// <summary>
    /// Get my certificates (Learner only)
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Learner")]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<CertificateResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyCertificates()
    {
        var result = await _certificateService.GetMyCertificatesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Issue certificate for a completed course (Learner only)
    /// </summary>
    [HttpPost("{courseId:guid}")]
    [Authorize(Roles = "Learner")]
    [ProducesResponseType(typeof(BaseResponse<CertificateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Issue(Guid courseId)
    {
        var result = await _certificateService.IssueCertificateAsync(courseId);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}
using LMS.Application.Common;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Profile;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Get the authenticated user's profile
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(BaseResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _profileService.GetMyProfileAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Update the authenticated user's profile
    /// </summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(BaseResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
    {
        var result = await _profileService.UpdateMyProfileAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Upload the authenticated user's profile photo
    /// </summary>
    [HttpPost("me/photo")]
    [ProducesResponseType(typeof(BaseResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadMyProfilePhoto(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(BaseResponse<string>.Fail("Profile photo file is required"));

        await using var stream = file.OpenReadStream();
        var request = new FileUploadRequest(
            stream,
            file.FileName,
            file.ContentType,
            string.Empty
        );

        var result = await _profileService.UploadMyProfilePhotoAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

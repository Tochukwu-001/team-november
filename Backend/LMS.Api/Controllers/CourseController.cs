using LMS.Application.Common;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Course;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(BaseResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var result = await _courseService.CreateCourseAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(BaseResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseRequest request)
    {
        var result = await _courseService.UpdateCourseAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:guid}/thumbnail")]
    [Authorize(Roles = "Admin,Instructor")]
    [ProducesResponseType(typeof(BaseResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadThumbnail(Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(BaseResponse<string>.Fail("Thumbnail file is required"));

        await using var stream = file.OpenReadStream();
        var request = new FileUploadRequest(
            stream,
            file.FileName,
            file.ContentType,
            string.Empty
        );

        var result = await _courseService.UploadThumbnailAsync(id, request, cancellationToken);
        return result.Success ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<CourseCatalogItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalog(
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] bool enrolledOnly = false)
    {
        var result = await _courseService.GetCatalogAsync(search, category, enrolledOnly);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<CourseDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var result = await _courseService.GetCourseDetailAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}

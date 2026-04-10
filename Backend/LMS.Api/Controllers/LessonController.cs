using LMS.Application.Common;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Lesson;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Create a new lesson (Instructor only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(BaseResponse<LessonResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateLessonRequest request)
    {
        var result = await _lessonService.CreateLessonAsync(request);

        return result.Success? Ok(result): result.Message.Contains("not found")? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Add content to a lesson (Instructor only)
    /// </summary>
    [HttpPost("content")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(BaseResponse<LessonResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddContent([FromBody] AddLessonContentRequest request)
    {
        var result = await _lessonService.AddContentAsync(request);

        return result.Success? Ok(result): result.Message.Contains("not found")? NotFound(result): BadRequest(result);
    }

    /// <summary>
    /// Upload a PDF and attach it to a lesson (Instructor only)
    /// </summary>
    [HttpPost("{lessonId:guid}/content/pdf")]
    [Authorize(Roles = "Instructor")]
    [ProducesResponseType(typeof(BaseResponse<LessonResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPdf(Guid lessonId, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(BaseResponse<string>.Fail("PDF file is required"));

        await using var stream = file.OpenReadStream();
        var request = new FileUploadRequest(
            stream,
            file.FileName,
            file.ContentType,
            string.Empty
        );

        var result = await _lessonService.UploadPdfContentAsync(lessonId, request, cancellationToken);
        return result.Success ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Get lessons by course (Authenticated users)
    /// </summary>
    [HttpGet("course/{courseId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<LessonResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCourseId(Guid courseId)
    {
        var result = await _lessonService.GetLessonsByCourseIdAsync(courseId);

        return result.Success? Ok(result): result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    [HttpGet("{lessonId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<LessonPlayerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayer(Guid lessonId)
    {
        var result = await _lessonService.GetLessonPlayerAsync(lessonId);

        return result.Success ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    [HttpPost("{lessonId:guid}/complete")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Complete(Guid lessonId)
    {
        var result = await _lessonService.CompleteLessonAsync(lessonId);

        return result.Success ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    [HttpPut("{lessonId:guid}/note")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<LessonNoteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveNote(Guid lessonId, [FromBody] SaveLessonNoteRequest request)
    {
        var result = await _lessonService.SaveLessonNoteAsync(lessonId, request);

        return result.Success ? Ok(result) : result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
}

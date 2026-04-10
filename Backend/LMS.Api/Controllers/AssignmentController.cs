using LMS.Application.Common;
using LMS.Application.DTOs.Assignment;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    /// <summary>
    /// Create a new assignment (Admin/Instructor only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request)
    {
        var result = await _assignmentService.CreateAssignmentAsync(request);

        if (!result.Success || result.Data == null)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetByCourseId),
            new { courseId = result.Data.CourseId },
            result);
    }

    /// <summary>
    /// Get assignments by course (Authenticated users)
    /// </summary>
    [HttpGet("course/{courseId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByCourseId(Guid courseId)
    {
        var result = await _assignmentService.GetAssignmentsByCourseIdAsync(courseId);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Learner")]
    public async Task<IActionResult> GetMyAssignments([FromQuery] string? status = null)
    {
        var result = await _assignmentService.GetMyAssignmentsAsync(status);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get paginated assignments
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _assignmentService.GetPagedAsync(page, pageSize);
        return Ok(result);
    }
}

using LMS.Application.Common;
using LMS.Application.DTOs.Discussion;
using LMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class DiscussionController : ControllerBase
{
    private readonly IDiscussionService _discussionService;

    public DiscussionController(IDiscussionService discussionService)
    {
        _discussionService = discussionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<DiscussionPostSummaryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts([FromQuery] string? tag = null, [FromQuery] string? search = null, [FromQuery] string? sort = null)
    {
        var result = await _discussionService.GetPostsAsync(tag, search, sort);
        return Ok(result);
    }

    [HttpGet("{postId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<DiscussionThreadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetThread(Guid postId)
    {
        var result = await _discussionService.GetThreadAsync(postId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<DiscussionThreadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDiscussionPostRequest request)
    {
        var result = await _discussionService.CreatePostAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{postId:guid}/reply")]
    [ProducesResponseType(typeof(BaseResponse<DiscussionReplyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reply(Guid postId, [FromBody] CreateDiscussionReplyRequest request)
    {
        var result = await _discussionService.ReplyAsync(postId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("top-contributors")]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<DiscussionContributorResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopContributors([FromQuery] int count = 5)
    {
        var result = await _discussionService.GetTopContributorsAsync(count);
        return Ok(result);
    }
}

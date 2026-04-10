using LMS.Application.Common;
using LMS.Application.DTOs.Discussion;

namespace LMS.Application.Interfaces.Services;

public interface IDiscussionService
{
    Task<BaseResponse<IEnumerable<DiscussionPostSummaryResponse>>> GetPostsAsync(string? tag, string? search, string? sort);
    Task<BaseResponse<DiscussionThreadResponse>> GetThreadAsync(Guid postId);
    Task<BaseResponse<DiscussionThreadResponse>> CreatePostAsync(CreateDiscussionPostRequest request);
    Task<BaseResponse<DiscussionReplyResponse>> ReplyAsync(Guid postId, CreateDiscussionReplyRequest request);
    Task<BaseResponse<IEnumerable<DiscussionContributorResponse>>> GetTopContributorsAsync(int count);
}

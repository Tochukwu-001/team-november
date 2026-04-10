using LMS.Application.DTOs.Discussion;
using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories;

public interface IDiscussionRepository
{
    Task<List<DiscussionPost>> GetPostsAsync(string? tag, string? search, string? sort);
    Task<DiscussionPost?> GetPostByIdAsync(Guid postId);
    Task<DiscussionTag?> GetTagByNameAsync(string name);
    Task AddPostAsync(DiscussionPost post);
    Task AddReplyAsync(DiscussionReply reply);
    Task AddTagAsync(DiscussionTag tag);
    Task AddPostTagAsync(DiscussionPostTag postTag);
    Task<List<DiscussionContributorResponse>> GetTopContributorsAsync(int count);
}

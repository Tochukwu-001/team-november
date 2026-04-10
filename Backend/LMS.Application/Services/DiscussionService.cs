using LMS.Application.Common;
using LMS.Application.DTOs.Discussion;
using LMS.Application.DTOs.Notification;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Services;

public class DiscussionService : IDiscussionService
{
    private readonly IDiscussionRepository _discussionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public DiscussionService(
        IDiscussionRepository discussionRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _discussionRepository = discussionRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<IEnumerable<DiscussionPostSummaryResponse>>> GetPostsAsync(string? tag, string? search, string? sort)
    {
        var posts = await _discussionRepository.GetPostsAsync(tag, search, sort);
        return BaseResponse<IEnumerable<DiscussionPostSummaryResponse>>.Ok(
            posts.Select(MapPostSummary).ToList());
    }

    public async Task<BaseResponse<DiscussionThreadResponse>> GetThreadAsync(Guid postId)
    {
        var post = await _discussionRepository.GetPostByIdAsync(postId);
        if (post == null)
            return BaseResponse<DiscussionThreadResponse>.Fail("Discussion post not found");

        return BaseResponse<DiscussionThreadResponse>.Ok(MapThread(post));
    }

    public async Task<BaseResponse<DiscussionThreadResponse>> CreatePostAsync(CreateDiscussionPostRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<DiscussionThreadResponse>.Fail("Unauthorized");

        var post = DiscussionPost.Create(_currentUserService.UserId.Value, request.Title, request.Content);
        await _discussionRepository.AddPostAsync(post);

        foreach (var rawTag in request.Tags
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Select(x => x.Trim())
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var tag = await _discussionRepository.GetTagByNameAsync(rawTag);
            if (tag == null)
            {
                tag = DiscussionTag.Create(rawTag);
                await _discussionRepository.AddTagAsync(tag);
            }

            await _discussionRepository.AddPostTagAsync(DiscussionPostTag.Create(post.Id, tag.Id));
        }

        await _unitOfWork.SaveChangesAsync();

        var savedPost = await _discussionRepository.GetPostByIdAsync(post.Id);
        return BaseResponse<DiscussionThreadResponse>.Ok(MapThread(savedPost!), "Discussion created successfully");
    }

    public async Task<BaseResponse<DiscussionReplyResponse>> ReplyAsync(Guid postId, CreateDiscussionReplyRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<DiscussionReplyResponse>.Fail("Unauthorized");

        var post = await _discussionRepository.GetPostByIdAsync(postId);
        if (post == null)
            return BaseResponse<DiscussionReplyResponse>.Fail("Discussion post not found");

        var reply = DiscussionReply.Create(postId, _currentUserService.UserId.Value, request.Content);
        post.AddReply(reply);
        await _discussionRepository.AddReplyAsync(reply);
        await _unitOfWork.SaveChangesAsync();

        if (post.UserId != _currentUserService.UserId.Value)
        {
            await _notificationService.NotifyUserAsync(new CreateNotificationRequest(
                post.UserId,
                NotificationType.DiscussionReply,
                "New reply on your discussion",
                $"Someone replied to \"{post.Title}\".",
                $"/discussion/{post.Id}",
                false));
        }

        var savedPost = await _discussionRepository.GetPostByIdAsync(postId);
        var savedReply = savedPost!.Replies.OrderByDescending(x => x.CreatedAt).First(x => x.Id == reply.Id);

        return BaseResponse<DiscussionReplyResponse>.Ok(MapReply(savedReply), "Reply added successfully");
    }

    public async Task<BaseResponse<IEnumerable<DiscussionContributorResponse>>> GetTopContributorsAsync(int count)
    {
        if (count <= 0)
            count = 5;

        var contributors = await _discussionRepository.GetTopContributorsAsync(count);
        return BaseResponse<IEnumerable<DiscussionContributorResponse>>.Ok(contributors);
    }

    private DiscussionThreadResponse MapThread(DiscussionPost post)
    {
        return new DiscussionThreadResponse(
            MapPostSummary(post),
            post.Replies
                .OrderBy(x => x.CreatedAt)
                .Select(MapReply)
                .ToList());
    }

    private DiscussionPostSummaryResponse MapPostSummary(DiscussionPost post)
    {
        return new DiscussionPostSummaryResponse(
            post.Id,
            post.Title,
            post.Content,
            post.UserId,
            post.User.FullName,
            post.User.PublicId,
            _currentUserService.UserId == post.UserId,
            post.CreatedAt,
            post.Replies.Count,
            post.PostTags
                .Select(x => new DiscussionTagResponse(x.Tag.Id, x.Tag.Name))
                .OrderBy(x => x.Name)
                .ToList());
    }

    private DiscussionReplyResponse MapReply(DiscussionReply reply)
    {
        return new DiscussionReplyResponse(
            reply.Id,
            reply.UserId,
            reply.User.FullName,
            reply.User.PublicId,
            _currentUserService.UserId == reply.UserId,
            reply.Content,
            reply.CreatedAt);
    }
}

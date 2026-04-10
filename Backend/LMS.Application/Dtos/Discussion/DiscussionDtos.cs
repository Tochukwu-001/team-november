namespace LMS.Application.DTOs.Discussion;

public record CreateDiscussionPostRequest(string Title, string Content, IReadOnlyCollection<string> Tags);
public record CreateDiscussionReplyRequest(string Content);

public record DiscussionTagResponse(Guid Id, string Name);

public record DiscussionPostSummaryResponse(
    Guid Id,
    string Title,
    string Content,
    Guid AuthorId,
    string AuthorName,
    string AuthorPublicId,
    bool IsCurrentUserAuthor,
    DateTime CreatedAt,
    int ReplyCount,
    IReadOnlyCollection<DiscussionTagResponse> Tags
);

public record DiscussionReplyResponse(
    Guid Id,
    Guid UserId,
    string AuthorName,
    string AuthorPublicId,
    bool IsCurrentUserAuthor,
    string Content,
    DateTime CreatedAt
);

public record DiscussionThreadResponse(
    DiscussionPostSummaryResponse Post,
    IReadOnlyCollection<DiscussionReplyResponse> Replies
);

public record DiscussionContributorResponse(
    Guid UserId,
    string FullName,
    string PublicId,
    int ContributionCount
);

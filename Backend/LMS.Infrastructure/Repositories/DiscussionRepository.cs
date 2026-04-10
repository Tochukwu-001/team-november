using LMS.Application.DTOs.Discussion;
using LMS.Application.Interfaces.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class DiscussionRepository : IDiscussionRepository
{
    private readonly ApplicationDbContext _context;

    public DiscussionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LMS.Domain.Entities.DiscussionPost>> GetPostsAsync(string? tag, string? search, string? sort)
    {
        var query = _context.Set<LMS.Domain.Entities.DiscussionPost>()
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Replies)
            .Include(x => x.PostTags)
                .ThenInclude(x => x.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var normalizedTag = tag.Trim().ToLowerInvariant();
            query = query.Where(x => x.PostTags.Any(pt => pt.Tag.Name.ToLower() == normalizedTag));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Title.ToLower().Contains(normalizedSearch) ||
                x.Content.ToLower().Contains(normalizedSearch));
        }

        query = string.Equals(sort, "trending", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(x => x.Replies.Count).ThenByDescending(x => x.CreatedAt)
            : query.OrderByDescending(x => x.CreatedAt);

        return await query
            .ToListAsync();
    }

    public async Task<LMS.Domain.Entities.DiscussionPost?> GetPostByIdAsync(Guid postId)
    {
        return await _context.Set<LMS.Domain.Entities.DiscussionPost>()
            .Include(x => x.User)
            .Include(x => x.Replies)
                .ThenInclude(x => x.User)
            .Include(x => x.PostTags)
                .ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(x => x.Id == postId);
    }

    public async Task<LMS.Domain.Entities.DiscussionTag?> GetTagByNameAsync(string name)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        return await _context.Set<LMS.Domain.Entities.DiscussionTag>()
            .FirstOrDefaultAsync(x => x.Name.ToLower() == normalizedName);
    }

    public async Task AddPostAsync(LMS.Domain.Entities.DiscussionPost post)
    {
        await _context.Set<LMS.Domain.Entities.DiscussionPost>().AddAsync(post);
    }

    public async Task AddReplyAsync(LMS.Domain.Entities.DiscussionReply reply)
    {
        await _context.Set<LMS.Domain.Entities.DiscussionReply>().AddAsync(reply);
    }

    public async Task AddTagAsync(LMS.Domain.Entities.DiscussionTag tag)
    {
        await _context.Set<LMS.Domain.Entities.DiscussionTag>().AddAsync(tag);
    }

    public async Task AddPostTagAsync(LMS.Domain.Entities.DiscussionPostTag postTag)
    {
        await _context.Set<LMS.Domain.Entities.DiscussionPostTag>().AddAsync(postTag);
    }

    public async Task<List<DiscussionContributorResponse>> GetTopContributorsAsync(int count)
    {
        var postCounts = await _context.Set<LMS.Domain.Entities.DiscussionPost>()
            .AsNoTracking()
            .GroupBy(x => x.UserId)
            .Select(x => new { UserId = x.Key, Count = x.Count() })
            .ToListAsync();

        var replyCounts = await _context.Set<LMS.Domain.Entities.DiscussionReply>()
            .AsNoTracking()
            .GroupBy(x => x.UserId)
            .Select(x => new { UserId = x.Key, Count = x.Count() })
            .ToListAsync();

        var totals = postCounts
            .Concat(replyCounts)
            .GroupBy(x => x.UserId)
            .Select(x => new { UserId = x.Key, Count = x.Sum(i => i.Count) })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToList();

        var userIds = totals.Select(x => x.UserId).ToList();
        var users = await _context.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        return totals
            .Where(x => users.ContainsKey(x.UserId))
            .Select(x => new DiscussionContributorResponse(
                x.UserId,
                users[x.UserId].FullName,
                users[x.UserId].PublicId,
                x.Count))
            .ToList();
    }
}

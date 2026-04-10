using System.Linq.Expressions;
using System.Reflection;
using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonContent> LessonContents => Set<LessonContent>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Progress> Progresses => Set<Progress>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<LessonNote> LessonNotes => Set<LessonNote>();
    public DbSet<DiscussionPost> DiscussionPosts => Set<DiscussionPost>();
    public DbSet<DiscussionReply> DiscussionReplies => Set<DiscussionReply>();
    public DbSet<DiscussionTag> DiscussionTags => Set<DiscussionTag>();
    public DbSet<DiscussionPostTag> DiscussionPostTags => Set<DiscussionPostTag>();
    public DbSet<InstructorRoleRequest> InstructorRoleRequests => Set<InstructorRoleRequest>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(CreateFilterExpression(entityType.ClrType));
        }
    }
    private static readonly Dictionary<Type, LambdaExpression> _filters = new();

    private static LambdaExpression CreateFilterExpression(Type entityType)
    {
        if (_filters.TryGetValue(entityType, out var filter))
            return filter;

        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false, typeof(bool)));

        var lambda = Expression.Lambda(condition, parameter);

        _filters[entityType] = lambda;

        return lambda;
    }
}

using LMS.Domain.Enums;

namespace LMS.Domain.Entities;

public class InstructorRoleRequest : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Bio { get; private set; } = default!;
    public string Expertise { get; private set; } = default!;
    public RoleRequestStatus Status { get; private set; } = RoleRequestStatus.Pending;
    public Guid? ReviewedByUserId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public User User { get; private set; } = default!;
    public User? ReviewedByUser { get; private set; }

    private InstructorRoleRequest() { }

    public static InstructorRoleRequest Create(Guid userId, string bio, string expertise)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (string.IsNullOrWhiteSpace(bio))
            throw new ArgumentException("Bio is required");

        if (string.IsNullOrWhiteSpace(expertise))
            throw new ArgumentException("Expertise is required");

        return new InstructorRoleRequest
        {
            UserId = userId,
            Bio = bio.Trim(),
            Expertise = expertise.Trim(),
            Status = RoleRequestStatus.Pending
        };
    }

    public void Approve(Guid adminUserId)
    {
        if (Status != RoleRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved");

        if (adminUserId == Guid.Empty)
            throw new ArgumentException("Admin user is required");

        Status = RoleRequestStatus.Approved;
        ReviewedByUserId = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = null;
        SetUpdated();
    }

    public void Reject(Guid adminUserId, string? rejectionReason)
    {
        if (Status != RoleRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected");

        if (adminUserId == Guid.Empty)
            throw new ArgumentException("Admin user is required");

        Status = RoleRequestStatus.Rejected;
        ReviewedByUserId = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        RejectionReason = string.IsNullOrWhiteSpace(rejectionReason) ? null : rejectionReason.Trim();
        SetUpdated();
    }
}

using LMS.Domain.Enums;

namespace LMS.Application.DTOs.Role;

public record RequestInstructorRoleRequest(
    string Bio,
    string Expertise
);

public record ReviewRoleRequest(
    Guid RoleRequestId,
    string? RejectionReason
);

public record AssignUserRoleRequest(
    Guid UserId,
    UserRole Role
);

public record InstructorRoleRequestResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string PublicId,
    string Bio,
    string Expertise,
    RoleRequestStatus Status,
    DateTime CreatedAt,
    Guid? ReviewedByUserId,
    string? ReviewedByName,
    DateTime? ReviewedAt,
    string? RejectionReason
);

public record UserRoleAssignmentResponse(
    Guid UserId,
    string FullName,
    string Email,
    UserRole Role
);

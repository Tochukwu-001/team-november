namespace LMS.Application.DTOs.Team;

public record TeamMemberResponse(
    Guid Id,
    string PublicId,
    string FullName,
    string Discipline,
    bool IsCurrentUser
);

public record TeamResponse(
    Guid Id,
    string Name,
    string Description,
    int MemberCount,
    IReadOnlyCollection<TeamMemberResponse> Members
);

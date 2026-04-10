namespace LMS.Application.DTOs.Profile;

public record UpdateProfileRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber
);

public record ProfileBadgeResponse(string Label);

public record PersonalInformationResponse(
    string FullName,
    string Email,
    string? PhoneNumber,
    string Discipline,
    string? CohortLabel,
    string? Location
);

public record LearningSummaryResponse(
    int Courses,
    double AverageProgress,
    int Certificates,
    decimal AverageScore
);

public record ProfileAchievementResponse(
    string Title,
    string Description
);

public record UserProfileResponse(
    Guid UserId,
    string FullName,
    string PublicId,
    string? ProfileImageUrl,
    string Headline,
    IReadOnlyCollection<ProfileBadgeResponse> Badges,
    PersonalInformationResponse PersonalInformation,
    LearningSummaryResponse LearningSummary,
    IReadOnlyCollection<ProfileAchievementResponse> Achievements
);

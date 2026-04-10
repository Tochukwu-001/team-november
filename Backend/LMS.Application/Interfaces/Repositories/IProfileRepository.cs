using LMS.Application.DTOs.Profile;

namespace LMS.Application.Interfaces.Repositories;

public interface IProfileRepository
{
    Task<UserProfileResponse?> GetProfileAsync(Guid userId);
}

using LMS.Application.Common;
using LMS.Application.Common.Options;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Profile;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace LMS.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly FileStorageOptions _fileStorageOptions;

    public ProfileService(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService,
        IOptions<FileStorageOptions> fileStorageOptions)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _fileStorageOptions = fileStorageOptions.Value;
    }

    public async Task<BaseResponse<UserProfileResponse>> GetMyProfileAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<UserProfileResponse>.Fail("Unauthorized");

        var profile = await _profileRepository.GetProfileAsync(_currentUserService.UserId.Value);
        if (profile == null)
            return BaseResponse<UserProfileResponse>.Fail("User not found");

        return BaseResponse<UserProfileResponse>.Ok(profile);
    }

    public async Task<BaseResponse<UserProfileResponse>> UpdateMyProfileAsync(UpdateProfileRequest request)
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<UserProfileResponse>.Fail("Unauthorized");

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value);
        if (user == null)
            return BaseResponse<UserProfileResponse>.Fail("User not found");

        user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        await _userRepository.UpdateAsync(user);

        var profile = await _profileRepository.GetProfileAsync(user.Id);
        if (profile == null)
            return BaseResponse<UserProfileResponse>.Fail("Profile not found");

        return BaseResponse<UserProfileResponse>.Ok(profile, "Profile updated successfully");
    }

    public async Task<BaseResponse<UserProfileResponse>> UploadMyProfilePhotoAsync(
        FileUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<UserProfileResponse>.Fail("Unauthorized");

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value);
        if (user == null)
            return BaseResponse<UserProfileResponse>.Fail("User not found");

        if (!request.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BaseResponse<UserProfileResponse>.Fail("Only image files are allowed");

        FileUploadResult uploadResult;
        try
        {
            var uploadRequest = request with { Folder = _fileStorageOptions.ProfileImageFolder };
            uploadResult = await _fileStorageService.UploadImageAsync(uploadRequest, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return BaseResponse<UserProfileResponse>.Fail(ex.Message);
        }

        user.UpdateProfileImage(uploadResult.Url);
        await _userRepository.UpdateAsync(user);

        var profile = await _profileRepository.GetProfileAsync(user.Id);
        if (profile == null)
            return BaseResponse<UserProfileResponse>.Fail("Profile not found");

        return BaseResponse<UserProfileResponse>.Ok(profile, "Profile photo uploaded successfully");
    }
}

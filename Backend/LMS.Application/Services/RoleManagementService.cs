using LMS.Application.Common;
using LMS.Application.DTOs.Notification;
using LMS.Application.DTOs.Role;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Services;

public class RoleManagementService : IRoleManagementService
{
    private readonly IInstructorRoleRequestRepository _roleRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public RoleManagementService(
        IInstructorRoleRequestRepository roleRequestRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService)
    {
        _roleRequestRepository = roleRequestRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<BaseResponse<InstructorRoleRequestResponse>> RequestInstructorRoleAsync(RequestInstructorRoleRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("Unauthorized");

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value);
        if (user == null || !user.IsActive)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("User not found");

        if (user.Role == UserRole.Admin || user.Role == UserRole.Instructor)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("Your current role does not require an instructor request");

        var existingPending = await _roleRequestRepository.GetPendingByUserIdAsync(user.Id);
        if (existingPending != null)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("You already have a pending instructor role request");

        var roleRequest = InstructorRoleRequest.Create(user.Id, request.Bio, request.Expertise);
        await _roleRequestRepository.AddAsync(roleRequest);

        var created = await _roleRequestRepository.GetByIdAsync(roleRequest.Id);
        return BaseResponse<InstructorRoleRequestResponse>.Ok(
            MapRequest(created!),
            "Instructor role request submitted successfully");
    }

    public async Task<BaseResponse<IEnumerable<InstructorRoleRequestResponse>>> GetRoleRequestsAsync(string? status)
    {
        RoleRequestStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<RoleRequestStatus>(status, true, out var statusValue))
                return BaseResponse<IEnumerable<InstructorRoleRequestResponse>>.Fail("Invalid status");

            parsedStatus = statusValue;
        }

        var requests = await _roleRequestRepository.GetAllWithUsersAsync(parsedStatus);
        return BaseResponse<IEnumerable<InstructorRoleRequestResponse>>.Ok(requests.Select(MapRequest));
    }

    public async Task<BaseResponse<InstructorRoleRequestResponse>> ApproveRoleRequestAsync(Guid roleRequestId)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("Unauthorized");

        var roleRequest = await _roleRequestRepository.GetByIdAsync(roleRequestId);
        if (roleRequest == null)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("Role request not found");

        var user = await _userRepository.GetByIdAsync(roleRequest.UserId);
        if (user == null)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("User not found");

        roleRequest.Approve(_currentUserService.UserId.Value);
        user.ChangeRole(UserRole.Instructor);

        await _roleRequestRepository.UpdateAsync(roleRequest);
        await _userRepository.UpdateAsync(user);

        await _notificationService.NotifyUserAsync(new CreateNotificationRequest(
            user.Id,
            NotificationType.System,
            "Role Request Approved",
            "Your instructor role request has been approved. Sign in again to refresh your access.",
            "/dashboard",
            false));

        var updated = await _roleRequestRepository.GetByIdAsync(roleRequest.Id);
        return BaseResponse<InstructorRoleRequestResponse>.Ok(
            MapRequest(updated!),
            "Role request approved successfully");
    }

    public async Task<BaseResponse<InstructorRoleRequestResponse>> RejectRoleRequestAsync(ReviewRoleRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("Unauthorized");

        var roleRequest = await _roleRequestRepository.GetByIdAsync(request.RoleRequestId);
        if (roleRequest == null)
            return BaseResponse<InstructorRoleRequestResponse>.Fail("Role request not found");

        roleRequest.Reject(_currentUserService.UserId.Value, request.RejectionReason);
        await _roleRequestRepository.UpdateAsync(roleRequest);

        await _notificationService.NotifyUserAsync(new CreateNotificationRequest(
            roleRequest.UserId,
            NotificationType.System,
            "Role Request Rejected",
            string.IsNullOrWhiteSpace(request.RejectionReason)
                ? "Your instructor role request has been rejected."
                : $"Your instructor role request was rejected: {request.RejectionReason}",
            "/profile",
            false));

        var updated = await _roleRequestRepository.GetByIdAsync(roleRequest.Id);
        return BaseResponse<InstructorRoleRequestResponse>.Ok(
            MapRequest(updated!),
            "Role request rejected successfully");
    }

    public async Task<BaseResponse<UserRoleAssignmentResponse>> AssignRoleAsync(AssignUserRoleRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            return BaseResponse<UserRoleAssignmentResponse>.Fail("User not found");

        user.ChangeRole(request.Role);
        await _userRepository.UpdateAsync(user);

        await _notificationService.NotifyUserAsync(new CreateNotificationRequest(
            user.Id,
            NotificationType.System,
            "Role Updated",
            $"Your account role has been updated to {request.Role}. Sign in again to refresh your access.",
            "/dashboard",
            false));

        return BaseResponse<UserRoleAssignmentResponse>.Ok(
            new UserRoleAssignmentResponse(user.Id, user.FullName, user.Email, user.Role),
            "User role updated successfully");
    }

    private static InstructorRoleRequestResponse MapRequest(InstructorRoleRequest request)
    {
        return new InstructorRoleRequestResponse(
            request.Id,
            request.UserId,
            request.User.FullName,
            request.User.Email,
            request.User.PublicId,
            request.Bio,
            request.Expertise,
            request.Status,
            request.CreatedAt,
            request.ReviewedByUserId,
            request.ReviewedByUser?.FullName,
            request.ReviewedAt,
            request.RejectionReason
        );
    }
}

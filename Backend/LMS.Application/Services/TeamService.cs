using LMS.Application.Common;
using LMS.Application.DTOs.Team;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ICurrentUserService _currentUserService;

    public TeamService(ITeamRepository teamRepository, ICurrentUserService currentUserService)
    {
        _teamRepository = teamRepository;
        _currentUserService = currentUserService;
    }

    public async Task<BaseResponse<IEnumerable<TeamResponse>>> GetTeamsAsync()
    {
        var currentUserId = _currentUserService.UserId;
        var teams = await _teamRepository.GetAllWithMembersAsync();

        var response = teams
            .OrderBy(team => team.Name)
            .Select(team => MapToResponse(team, currentUserId))
            .ToList();

        return BaseResponse<IEnumerable<TeamResponse>>.Ok(response);
    }

    public async Task<BaseResponse<TeamResponse>> GetMyTeamAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<TeamResponse>.Fail("Unauthorized");

        var team = await _teamRepository.GetByMemberIdAsync(_currentUserService.UserId.Value);

        if (team == null)
            return BaseResponse<TeamResponse>.Fail("User is not assigned to a team");

        return BaseResponse<TeamResponse>.Ok(MapToResponse(team, _currentUserService.UserId));
    }

    private static TeamResponse MapToResponse(Team team, Guid? currentUserId)
    {
        var members = team.Members
            .OrderBy(member => member.FirstName)
            .ThenBy(member => member.LastName)
            .Select(member => new TeamMemberResponse(
                member.Id,
                member.PublicId,
                member.FullName,
                member.Discipline,
                currentUserId.HasValue && member.Id == currentUserId.Value
            ))
            .ToList();

        return new TeamResponse(
            team.Id,
            team.Name,
            team.Description,
            members.Count,
            members
        );
    }
}

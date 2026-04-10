using LMS.Application.Common;
using LMS.Application.DTOs.Team;

namespace LMS.Application.Interfaces.Services;

public interface ITeamService
{
    Task<BaseResponse<IEnumerable<TeamResponse>>> GetTeamsAsync();
    Task<BaseResponse<TeamResponse>> GetMyTeamAsync();
}

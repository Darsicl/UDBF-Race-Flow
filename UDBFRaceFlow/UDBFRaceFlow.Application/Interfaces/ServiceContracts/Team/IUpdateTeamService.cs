using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Team;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Team
{
    public interface IUpdateTeamService
    {
        Task<Result> UpdateTeamName(UpdateTeamDto teamDto, CancellationToken cancellationToken);
    }
}

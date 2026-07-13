using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Team;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Team
{
    public interface IDeleteTeamService
    {
        Task<Result> DeleteTeam(DeleteTeamDto teamDto, CancellationToken cancellationToken);
    }
}

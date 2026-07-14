using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Team;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Team
{
    public interface ICreateTeamService
    {
        Task<Result> CreateNewTeam(CreateTeamDto teamDto, CancellationToken cancellationToken);
    }
}
using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update
{
    public interface IUpdateStartLaneNumberService
    {
        Task<Result> ChangeTeamStartLane(UpdateStartLaneDto startLaneDto, CancellationToken cancellationToken);
    }
}

using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update
{
    public interface IUpdateRaceService
    {
        Task<Result> UpdateRaceStatus(UpdateStatusDto statusDto, CancellationToken cancellationToken);
        Task<Result> UpdateRaceDelay(RaceDelayDto raceDelayDto, CancellationToken cancellationToken);
        Task<Result> UpdateRaceDetails(UpdateRaceDetailsDto raceDetailsDto, CancellationToken cancellationToken);
    }
}

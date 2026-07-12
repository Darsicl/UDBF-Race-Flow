using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Update
{
    public interface IUpdateRaceService
    {
        Task<Result> UpdateRaceStatus(UpdateStatusDto statusDto, CancellationToken cancellationToken);
        Task<Result> UpdateRaceDelay(RaceDelayDto raceDelayDto, CancellationToken cancellationToken);
        Task<Result> UpdateRaceDetails(UpdateRaceDetailsDto raceDetailsDto, CancellationToken cancellationToken);
    }
}

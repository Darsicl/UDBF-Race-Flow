using FluentResults;
using UDBFRaceFlow.Application.Dto.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface IUpdateRaceService
    {
        Task<Result> UpdateRaceStatus(UpdateStatusDto statusDto, CancellationToken cancellationToken);
        Task<Result> UpdateLaneResult(UpdateLaneResultDto laneResultDto, CancellationToken cancellationToken);
        Task<Result> UpdateRaceDelay(RaceDelayDto raceDelayDto, CancellationToken cancellationToken);
        Task<Result> UpdateCategoryDetails(UpdateCategoryDetailsDto categoryDetailsDto, CancellationToken cancellationToken);
        Task<Result> UpdateRaceDetails(UpdateRaceDetailsDto raceDetailsDto, CancellationToken cancellationToken);
    }
}

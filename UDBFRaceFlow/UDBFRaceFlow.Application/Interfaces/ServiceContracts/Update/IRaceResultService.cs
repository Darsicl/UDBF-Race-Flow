using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Update
{
    public interface IRaceResultService
    {
        Task<Result> UpdateLaneResult(UpdateLaneResultDto laneResultDto, CancellationToken cancellationToken = default);
    }
}
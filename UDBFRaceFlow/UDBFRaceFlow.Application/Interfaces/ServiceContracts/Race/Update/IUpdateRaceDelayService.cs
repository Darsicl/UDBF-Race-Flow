using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update
{
    public interface IUpdateRaceDelayService
    {
        Task<Result> UpdateRaceDelay(RaceDelayDto raceDelayDto, CancellationToken cancellationToken);
    }
}

using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update
{
    public interface IUpdateRaceDetailsService
    {
        Task<Result> UpdateRaceDetails(UpdateRaceDetailsDto raceDetailsDto, CancellationToken cancellationToken);
    }
}

using FluentResults;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface ISystemGenerator
    {
        bool ApplyParametrs(int raceType, RaceSystems raceSystems);
        Task<Result> BuildGridAsync(CreateFullGridDto fullGridDto, CancellationToken cancellationToken);
        Task<Result> BuildSemifinalAsync(Guid categoryId, CancellationToken cancellationToken);
        Task<Result> BuildFinalAsync(Guid categoryId, CancellationToken cancellationToken);
    }
}

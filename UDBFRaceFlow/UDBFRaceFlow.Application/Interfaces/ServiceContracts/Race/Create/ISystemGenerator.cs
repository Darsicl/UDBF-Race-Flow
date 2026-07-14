using FluentResults;
using UDBFRaceFlow.Application.Dto.Request.Race.Create;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Create
{
    public interface ISystemGenerator
    {
        bool ApplyParametrs(int raceType, RaceSystem raceSystem);
        Task<Result> BuildGridAsync(CreateCategoryDto fullGridDto, CancellationToken cancellationToken);
        Task<Result> BuildSemifinalAsync(Guid categoryId, CancellationToken cancellationToken);
        Task<Result> BuildFinalAsync(Guid categoryId, CancellationToken cancellationToken);
    }
}

using FluentResults;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface ISystemGenerator
    {
        bool ApplyParametrs(int distance, RaceSystems raceSystems);
        Task<Result> BuildGrid(CreateFullGridDto fullGridDto);
        Task<Result> BuildSemifinal(Guid categoryId);
        Task<Result> BuildFinal(Guid categoryId);
    }
}

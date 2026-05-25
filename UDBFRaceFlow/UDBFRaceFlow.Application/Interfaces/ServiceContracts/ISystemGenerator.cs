using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface ISystemGenerator
    {
        RaceSystems raceSystem { get; }
        Task BuildGrid(CreateFullGridDto fullGridDto);
        Task BuildSemifinal(Guid categoryId);
        Task BuildFinal(Guid categoryId);
    }
}

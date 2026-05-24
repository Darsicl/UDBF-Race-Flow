using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Interfaces.ServiceContracts
{
    public interface ISystemGenerator
    {
        public RaceSystems raceSystem { get; }
        public Task BuildGrid(CreateSystemDto createSystemDto);
    }
}

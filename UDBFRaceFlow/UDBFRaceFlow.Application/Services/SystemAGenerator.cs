using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces;
using UDBFRaceFlow.Application.Interfaces.;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Services
{
    public class SystemAGenerator : ISystemGenerator
    {
        private readonly IRaceRepository _raceRepository;

        public SystemAGenerator(IRaceRepository raceRepository)
        {
            _raceRepository = raceRepository;
        }

        public RaceSystems raceSystem => RaceSystems.SystemA;

        public async Task BuildGrid(CreateSystemDto createSystemDto)
        {
            RaceData raceHeat = new RaceData
            {
                Id = Guid.NewGuid(),
                Distance = createSystemDto.Distance,
                BoatSize = createSystemDto.BoatSize,
                GenderCategory = createSystemDto.GenderCategory,
                RaceNumber = createSystemDto.RaceNumber,
                RaceStatus = RaceStatus.Scheduled,
                RaceType = RaceType.Heat,
                RaceEntries = createSystemDto.RaceEntries,
            };

            await _raceRepository.AddAsync(raceHeat);
        }
    }
}

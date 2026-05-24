using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces;
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

        public async Task BuildGrid(CreateFullGridDto fullGridDto)
        {
            foreach (RaceCreationDto raceDto in fullGridDto.Races)
            {

                RaceData firstHeat = new RaceData
                {
                    Id = Guid.NewGuid(),
                    BoatSize = fullGridDto.BoatSize,
                    Distance = fullGridDto.Distance,
                    GenderCategory = fullGridDto.Gender,
                    RaceStatus = RaceStatus.Scheduled,
                    RaceType = raceDto.RaceType,
                    RaceNumber = raceDto.RaceNumber,
                    RaceTime = raceDto.RaceTime
                };

                await _raceRepository.AddAsync(firstHeat);
            }


        }
    }
}

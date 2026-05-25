using Mapster;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Services.SystemA
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
            List<RaceCreationDto> sortRaces = fullGridDto.Races
                .OrderBy(r => r.RaceType)
                .ThenBy(r => r.SequenceNumber)
                .ToList();

            foreach (RaceCreationDto raceDto in sortRaces)
            {

                RaceData race = raceDto.Adapt<RaceData>();

                race.BoatSize = fullGridDto.BoatSize;
                race.GenderCategory = fullGridDto.Gender;
                race.Distance = fullGridDto.Distance;
                race.RaceStatus = RaceStatus.Scheduled;


                foreach (RaceEntry entry in race.RaceEntries)
                {
                    entry.RaceId = race.Id;
                }

                await _raceRepository.AddAsync(race);

            }

            await _raceRepository.SaveChangesAsync();

        }

        public async Task BuildSemifinal()
        {
            object heats =
        }


    }
}

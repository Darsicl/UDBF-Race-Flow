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
            RaceCategory category = fullGridDto.Adapt<RaceCategory>();

            List<RaceCreationDto> sortRaces = fullGridDto.Races
                .OrderBy(r => r.RaceType)
                .ThenBy(r => r.SequenceNumber)
                .ToList();

            foreach (RaceCreationDto raceDto in sortRaces)
            {

                RaceData race = raceDto.Adapt<RaceData>();
                race.RaceStatus = RaceStatus.Scheduled;


                foreach (LaneData entry in race.Lanes)
                {
                    entry.RaceId = race.Id;
                }

                await _raceRepository.AddRaceAsync(race);

            }

            await _raceRepository.AddCategoryAsync(category);

            await _raceRepository.SaveChangesAsync();

        }

        public async Task BuildSemifinal()
        {

        }


    }
}

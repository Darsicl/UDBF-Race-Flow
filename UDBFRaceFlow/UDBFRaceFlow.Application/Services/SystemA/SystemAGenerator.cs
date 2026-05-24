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
                .OrderBy(r => r.RaceNumber)
                .ToList();

            foreach (RaceCreationDto raceDto in fullGridDto.Races)
            {

                RaceData race = new RaceData
                {
                    Id = Guid.NewGuid(),
                    BoatSize = fullGridDto.BoatSize,
                    Distance = fullGridDto.Distance,
                    GenderCategory = fullGridDto.Gender,
                    RaceStatus = RaceStatus.Scheduled,
                    RaceType = raceDto.RaceType,
                    RaceNumber = raceDto.RaceNumber,
                    RaceTime = raceDto.RaceTime,
                    SequenceNumber = raceDto.SequenceNumber,
                    RaceEntries = new List<RaceEntry>()
                };

                foreach (LaneAssignmentDto raceEntry in raceDto.Lanes)
                {
                    RaceEntry entry = new RaceEntry
                    {
                        Id = Guid.NewGuid(),
                        TeamId = raceEntry.TeamId,
                        StartLane = raceEntry.StartLane,
                        RaceId = race.Id
                    };

                    race.RaceEntries.Add(entry);
                }

                await _raceRepository.AddAsync(race);
            }


        }
    }
}

using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.SystemA
{
    public class SystemAGenerator : ISystemGenerator
    {
        private readonly IRaceRepository _raceRepository;
        private readonly ILogger<SystemAGenerator> _logger;

        public SystemAGenerator(IRaceRepository raceRepository, ILogger<SystemAGenerator> logger)
        {
            _raceRepository = raceRepository;
            _logger = logger;
        }

        public RaceSystems raceSystem => RaceSystems.SystemA;

        public async Task BuildGrid(CreateFullGridDto fullGridDto)
        {
            RaceCategory category = fullGridDto.Adapt<RaceCategory>();
            category.Races = new List<RaceData>();

            List<RaceCreationDto> sortRaces = fullGridDto.Races
                .OrderBy(r => r.RaceType)
                .ThenBy(r => r.SequenceNumber)
                .ToList();

            foreach (RaceCreationDto raceDto in sortRaces)
            {
                RaceData race = raceDto.Adapt<RaceData>();

                race.RaceStatus = RaceStatus.Scheduled;

                race.CategoryId = category.Id;


                foreach (LaneData lane in race.Lanes)
                {
                    lane.RaceId = race.Id;
                }

                category.Races.Add(race);
            }

            await _raceRepository.AddCategoryAsync(category);

            await _raceRepository.SaveChangesAsync();

        }

        public async Task<Result> BuildSemifinal(Guid categoryId)
        {
            var category = await _raceRepository.GetCategory(categoryId);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var semi = category.Races.FirstOrDefault(s => s.RaceType == RaceType.Semifinal);

            var firstHeat = category.Races
                .Where(f => f.RaceType == RaceType.Heat)
                .FirstOrDefault(f => f.SequenceNumber == 1);

            var secondHeat = category.Races
                .Where(f => f.RaceType == RaceType.Heat)
                .FirstOrDefault(f => f.SequenceNumber == 2);

            var laneOneTeam = firstHeat.Lanes
                .FirstOrDefault(f => f.FinishPlace == 3);
            var laneTwoTeam = secondHeat.Lanes
                .FirstOrDefault(f => f.FinishPlace == 2);
            var laneTreeTeam = firstHeat.Lanes
                .FirstOrDefault(f => f.FinishPlace == 2);
            var laneFourTeam = secondHeat.Lanes
                .FirstOrDefault(f => f.FinishPlace == 3);

            semi.Lanes = new List<LaneData>
            {
                new LaneData {StartLane = 1, TeamId = laneOneTeam.Id, RaceId = semi.Id},
                new LaneData {StartLane = 2, TeamId = laneTwoTeam.Id, RaceId = semi.Id},
                new LaneData {StartLane = 3, TeamId = laneTreeTeam.Id, RaceId = semi.Id},
                new LaneData {StartLane = 4, TeamId = laneFourTeam.Id, RaceId = semi.Id}
            };


        }

        public async Task BuildFinal(Guid categoryId)
        {
            throw new NotImplementedException();
        }

    }
}

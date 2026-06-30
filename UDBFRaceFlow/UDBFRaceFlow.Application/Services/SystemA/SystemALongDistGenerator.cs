using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.SystemA
{
    public class SystemALongDistGenerator : ISystemGenerator
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly ILogger<SystemALongDistGenerator> _logger;

        public SystemALongDistGenerator(IRaceCategoryRepository raceRepository, ILogger<SystemALongDistGenerator> logger)
        {
            _raceCategoryRepository = raceRepository;
            _logger = logger;
        }
        public bool ApplyParametrs(int systemType, RaceSystems raceSystems)
        {
            return raceSystems == RaceSystems.SystemA && systemType == 1;
        }

        public async Task<Result> BuildGrid(CreateFullGridDto fullGridDto)
        {
            RaceCategory category = fullGridDto.Adapt<RaceCategory>();

            _logger.LogInformation(Messages.Info_StartGeneratingGrid, category.Id);

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

            var count = category.Races.Count();

            await _raceCategoryRepository.AddAsync(category);

            await _raceCategoryRepository.SaveChangesAsync();

            _logger.LogInformation(Messages.Info_FinishGenerateGrid, category.Id);

            return Result.Ok();

        }

        public async Task<Result> BuildSemifinal(Guid categoryId)
        {
            _logger.LogInformation(Messages.Info_StartBuildRace, nameof(RaceType.Semifinal), categoryId);

            RaceCategory? category = await _raceCategoryRepository.GetCategoryWithRacesAndLanesAsync(categoryId);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var semi = category.Races.FirstOrDefault(s => s.RaceType == RaceType.Semifinal);

            if (semi is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceType.Semifinal), category.Id);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var secondPlacesFromHeat = GetSortedLanesFromHeats(category, 2);

            var thirdPlacesFromHeat = GetSortedLanesFromHeats(category, 3);

            if (secondPlacesFromHeat.Count < 2 || thirdPlacesFromHeat.Count < 2)
            {
                string ErrMsg = string.Format(Messages.Error_NotEnoughHeatResults);
                return Result.Fail(ErrMsg);
            }

            LaneData? laneOneTeam = thirdPlacesFromHeat[0];
            LaneData? laneTwoTeam = secondPlacesFromHeat[0];
            LaneData? laneThreeTeam = secondPlacesFromHeat[1];
            LaneData? laneFourTeam = thirdPlacesFromHeat[1];

            semi.Lanes.Clear();

            semi.Lanes.Add(new LaneData { StartLane = 1, TeamId = laneOneTeam.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 2, TeamId = laneTwoTeam.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 3, TeamId = laneThreeTeam.TeamId, RaceId = semi.Id });
            semi.Lanes.Add(new LaneData { StartLane = 4, TeamId = laneFourTeam.TeamId, RaceId = semi.Id });

            await _raceCategoryRepository.SaveChangesAsync();

            _logger.LogInformation(Messages.Info_RaceWasBuilded, semi.RaceType, laneOneTeam.TeamId, laneTwoTeam.TeamId, laneThreeTeam.TeamId, laneFourTeam.TeamId);

            return Result.Ok();
        }

        public async Task<Result> BuildFinal(Guid categoryId)
        {
            _logger.LogInformation(Messages.Info_StartBuildRace, nameof(RaceType.Final), categoryId);

            RaceCategory? category = await _raceCategoryRepository.GetCategoryWithRacesAndLanesAsync(categoryId);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            RaceData? final = category.Races.FirstOrDefault(s => s.RaceType == RaceType.Final);

            if (final is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceType.Final), category.Id);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var firstPlacesFromHeat = GetSortedLanesFromHeats(category, 1);

            var semi = category.Races.
                Where(s => s.RaceType == RaceType.Semifinal)
                .SelectMany(s => s.Lanes)
                .ToList();

            if (firstPlacesFromHeat.Count < 2)
            {
                string ErrMsg = string.Format(Messages.Error_NotEnoughHeatResults);
                return Result.Fail(ErrMsg);
            }

            final.Lanes.Clear();

            var laneOneTeam = semi
                .FirstOrDefault(s => s.FinishPlace == 1);
            var laneTwoTeam = firstPlacesFromHeat[0];
            var laneThreeTeam = firstPlacesFromHeat[1];
            var laneFourTeam = semi
                .FirstOrDefault(s => s.FinishPlace == 2);

            if (laneFourTeam is null || laneOneTeam is null)
            {
                string ErrMsg = string.Format(Messages.Error_NotEnoughSemisResults);
                return Result.Fail(ErrMsg);
            }

            final.Lanes.Add(new LaneData { StartLane = 1, TeamId = laneOneTeam.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 2, TeamId = laneTwoTeam.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 3, TeamId = laneThreeTeam.TeamId, RaceId = final.Id });
            final.Lanes.Add(new LaneData { StartLane = 4, TeamId = laneFourTeam.TeamId, RaceId = final.Id });

            await _raceCategoryRepository.SaveChangesAsync();

            _logger.LogInformation(Messages.Info_RaceWasBuilded, final.RaceType, laneOneTeam.TeamId, laneTwoTeam.TeamId, laneThreeTeam.TeamId, laneFourTeam.TeamId);

            return Result.Ok();
        }

        private List<LaneData> GetSortedLanesFromHeats(RaceCategory category, int finishPlace)
        {
            return category.Races
                .Where(f => f.RaceType == RaceType.Heat)
                .SelectMany(f => f.Lanes)
                .Where(f => f.FinishPlace == finishPlace)
                .OrderBy(f => f.FinishTime)
                .ToList();
        }

    }
}

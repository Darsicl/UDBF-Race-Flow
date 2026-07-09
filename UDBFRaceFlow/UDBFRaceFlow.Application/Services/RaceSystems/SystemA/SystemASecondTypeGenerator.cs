using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.RaceSystems.SystemA
{
    public class SystemASecondTypeGenerator : ISystemGenerator
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly IRaceDataRepository _raceRepository;
        private readonly ILogger<SystemASecondTypeGenerator> _logger;
        private readonly SystemADtoValidator _validator;

        public SystemASecondTypeGenerator(IRaceCategoryRepository raceCategoryRepository, ILogger<SystemASecondTypeGenerator> logger, IRaceDataRepository raceRepository, SystemADtoValidator validator)
        {
            _raceCategoryRepository = raceCategoryRepository;
            _logger = logger;
            _raceRepository = raceRepository;
            _validator = validator;
        }
        public bool ApplyParametrs(int systemType, RaceSystem raceSystem)
        {
            return raceSystem == RaceSystem.SystemA && systemType == 2;
        }

        public async Task<Result> BuildGridAsync(CreateCategoryDto fullGridDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(fullGridDto, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMsg = string.Join(",", validationResult.Errors.Select(v => v.ErrorMessage));
                _logger.LogWarning(string.Format(Messages.Error_ValidationFailed, fullGridDto.RaceSystem, errorMsg));
                return Result.Fail(new Error(errorMsg));
            }

            RaceCategory category = fullGridDto.Adapt<RaceCategory>();

            _logger.LogInformation(Messages.Info_StartGeneratingGrid, category.Id);

            List<CreateRaceDto> sortRaces = fullGridDto.Races
                .OrderBy(r => r.RaceType)
                .ThenBy(r => r.SequenceNumber)
                .ToList();

            foreach (CreateRaceDto raceDto in sortRaces)
            {
                RaceData race = raceDto.Adapt<RaceData>();

                race.RaceStatus = RaceStatus.Scheduled;
                race.OriginalDateTime = race.RaceTime;

                foreach (LaneData laneDto in race.Lanes)
                {
                    var lane = laneDto.Adapt<LaneData>();
                    race.AddLane(lane);
                }

                category.AddRace(race);
            }

            var existingRaces = await _raceRepository.GetAllRacesAsync(cancellationToken);
            var allRaces = existingRaces.Concat(category.Races).ToList();
            var check = CheckIntervalTimeExtension.CheckInterval(allRaces);

            if (!check)
            {
                string errorMsg = string.Format(Messages.Error_CheckIntervalFail, category.CategoryName);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _raceCategoryRepository.AddAsync(category, cancellationToken);

            await _raceCategoryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(Messages.Info_FinishGenerateGrid, category.Id);

            return Result.Ok();

        }

        public async Task<Result> BuildSemifinalAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(Messages.Info_StartBuildRace, nameof(RaceType.Semifinal), categoryId);

            RaceCategory? category = await _raceCategoryRepository.GetCategoryWithRacesAndLanesAsync(categoryId, cancellationToken);

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

            await _raceCategoryRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(Messages.Info_RaceWasBuilded, semi.RaceType, laneOneTeam.TeamId, laneTwoTeam.TeamId, laneThreeTeam.TeamId, laneFourTeam.TeamId);

            return Result.Ok();
        }

        public async Task<Result> BuildFinalAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(Messages.Info_StartBuildRace, nameof(RaceType.Final), categoryId);

            RaceCategory? category = await _raceCategoryRepository.GetCategoryWithRacesAndLanesAsync(categoryId, cancellationToken);

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

            await _raceCategoryRepository.SaveChangesAsync(cancellationToken);

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

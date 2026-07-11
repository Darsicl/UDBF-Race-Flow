using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Update
{
    public class UpdateRaceService : IUpdateRaceService
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly IRaceDataRepository _raceDataRepository;
        private readonly ILogger<UpdateRaceService> _logger;

        public UpdateRaceService(IRaceCategoryRepository raceCategoryRepository, ILogger<UpdateRaceService> logger, IRaceDataRepository raceDataRepository)
        {
            _raceCategoryRepository = raceCategoryRepository;
            _logger = logger;
            _raceDataRepository = raceDataRepository;
        }

        public async Task<Result> UpdateCategoryDetails(UpdateCategoryDetailsDto categoryDetailsDto, CancellationToken cancellationToken = default)
        {
            var category = await _raceCategoryRepository.GetByIdAsync(categoryDetailsDto.CategoryId, cancellationToken);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryDetailsDto.CategoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            categoryDetailsDto.Adapt(category);

            var checkCategory = await _raceCategoryRepository.IsCategoryUnique(category, cancellationToken);

            if (checkCategory)
            {
                string errorMsg = string.Format(Messages.Error_PropertyNotUnique, category.CategoryName);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _raceCategoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        public async Task<Result> UpdateLaneResult(UpdateLaneResultDto laneResultDto, CancellationToken cancellationToken = default)
        {
            var race = await _raceDataRepository.GetRaceWithLanesAsync(laneResultDto.RaceId, cancellationToken);

            if (race is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceData), laneResultDto.RaceId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var laneResults = laneResultDto.LaneResults.ToDictionary(x => x.LaneId);

            foreach (var lane in race.Lanes)
            {
                if (laneResults.TryGetValue(lane.Id, out var dto))
                {
                    dto.Adapt(lane);
                }
            }

            race.RaceStatus = laneResultDto.RaceStatus;

            if (laneResultDto.RaceStatus == RaceStatus.Finished)
            {
                foreach (var lane in race.Lanes)
                {
                    lane.FinishPlace = null;
                }

                var sortLane = race.Lanes
                    .Where(s => s.FinishTime > TimeSpan.Zero && s.FinishTime.HasValue && s.FinishStatus == FinishStatus.Confirmed)
                    .OrderBy(s => s.FinishTime)
                    .ToList();

                for (int i = 0; i < sortLane.Count; i++)
                {
                    sortLane[i].FinishPlace = i + 1;
                }

                race.RaceStatus = laneResultDto.RaceStatus;
            }

            await _raceDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        public async Task<Result> UpdateRaceDelay(RaceDelayDto raceDelayDto, CancellationToken cancellationToken = default)
        {
            var date = raceDelayDto.DelayDay.ToDateTime(TimeOnly.MinValue);
            var races = await _raceDataRepository.GetRacesByDayForDelayAsync(date, cancellationToken);

            if (!races.Any())
            {
                return Result.Ok();
            }

            races.ForEach(r => r.RaceTime = r.OriginalDateTime + raceDelayDto.Delay);

            await _raceDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        public Task<Result> UpdateRaceDetails(UpdateRaceDetailsDto raceDetailsDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateRaceStatus(UpdateStatusDto statusDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

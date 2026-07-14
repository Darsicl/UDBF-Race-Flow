using FluentResults;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update.RaceResult
{
    public class RaceResultService : IRaceResultService
    {
        private readonly IRaceDataRepository _raceDataRepository;
        private readonly ILogger<RaceResultService> _logger;
        private readonly IValidator<UpdateLaneResultDto> _validator;
        public RaceResultService(IRaceDataRepository raceDataRepository, ILogger<RaceResultService> logger, IValidator<UpdateLaneResultDto> validator)
        {
            _raceDataRepository = raceDataRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> UpdateLaneResult(UpdateLaneResultDto laneResultDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateDtoAsync(laneResultDto, cancellationToken);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

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
            }

            await _raceDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

    }
}

using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update.StartLane
{
    public class UpdateStartLaneNumberService : IUpdateStartLaneNumberService
    {
        public readonly IRaceDataRepository _raceDataRepository;
        public readonly ILogger<UpdateStartLaneNumberService> _logger;
        public readonly IValidator<UpdateStartLaneDto> _validator;

        public UpdateStartLaneNumberService(IRaceDataRepository raceDataRepository, ILogger<UpdateStartLaneNumberService> logger, IValidator<UpdateStartLaneDto> validator)
        {
            _raceDataRepository = raceDataRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> ChangeTeamStartLane(UpdateStartLaneDto startLaneDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateDtoAsync(startLaneDto, cancellationToken);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var race = await _raceDataRepository.GetRaceWithLanesAsync(startLaneDto.raceId, cancellationToken);
            if (race is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceData), startLaneDto.raceId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            if (race.RaceStatus != RaceStatus.Scheduled)
            {
                string errorMsg = string.Format(Messages.Error_EditIsForbidden, race.RaceNumber);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var draggedLane = race.Lanes.FirstOrDefault(d => d.Id == startLaneDto.draggetLaneId);
            var targetLane = race.Lanes.FirstOrDefault(t => t.Id == startLaneDto.targetTeamId);

            if (targetLane is null || draggedLane is null)
            {
                string errorMsg = string.Format(Messages.Error_LaneIsNull, race.RaceNumber);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var startNum = draggedLane.StartLane;
            var endNum = targetLane.StartLane;

            if (startNum == endNum)
            {
                return Result.Ok();
            }

            var tempTeamId = draggedLane.TeamId;

            if (startNum < endNum)
            {
                var sortLane = race.Lanes
                    .Where(s => s.StartLane >= startNum && s.StartLane <= endNum)
                    .OrderBy(s => s.StartLane)
                    .ToList();

                for (int i = 0; i < sortLane.Count - 1; i++)
                {
                    sortLane[i].ChangeTeam(sortLane[i + 1].TeamId);
                }
            }
            else
            {
                var sortLane = race.Lanes
                    .Where(s => s.StartLane <= startNum && s.StartLane >= endNum)
                    .OrderByDescending(s => s.StartLane)
                    .ToList();

                for (int i = 0; i < sortLane.Count - 1; i++)
                {
                    sortLane[i].ChangeTeam(sortLane[i + 1].TeamId);

                }
            }

            targetLane.ChangeTeam(tempTeamId);

            await _raceDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

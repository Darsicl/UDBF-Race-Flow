using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update
{
    public class UpdateRaceService : IUpdateRaceService
    {
        private readonly IRaceDataRepository _raceDataRepository;
        private readonly ILogger<UpdateRaceService> _logger;

        public UpdateRaceService(ILogger<UpdateRaceService> logger, IRaceDataRepository raceDataRepository)
        {
            _logger = logger;
            _raceDataRepository = raceDataRepository;
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

        public async Task<Result> UpdateRaceDetails(UpdateRaceDetailsDto raceDetailsDto, CancellationToken cancellationToken = default)
        {
            var race = await _raceDataRepository.GetByIdAsync(raceDetailsDto.RaceId, cancellationToken);

            if (race is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceData), raceDetailsDto.RaceId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            raceDetailsDto.Adapt(race);

            var checkDate = await _raceDataRepository.IsRaceDateUniqueAsync(race.OriginalDateTime, race.Id, cancellationToken);

            if (checkDate)
            {
                string errorMsg = string.Format(Messages.Error_CheckIntervalFail, race.Id);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var checkRaceNumber = await _raceDataRepository.IsRaceNumberUniqueAsync(race.RaceNumber, race.Id, cancellationToken);

            if (checkRaceNumber)
            {
                string errorMsg = string.Format(Messages.Error_PropertyNotUnique, race.RaceNumber);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var allRaces = await _raceDataRepository.GetAllRacesAsync(cancellationToken);
            var check = CheckIntervalTimeExtension.CheckInterval(allRaces);

            if (!check)
            {
                string errorMsg = string.Format(Messages.Error_CheckIntervalFail, race.Id);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _raceDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        public async Task<Result> UpdateRaceStatus(UpdateStatusDto statusDto, CancellationToken cancellationToken = default)
        {
            var race = await _raceDataRepository.GetByIdAsync(statusDto.RaceId, cancellationToken);

            if (race is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceData), statusDto.RaceId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            race.RaceStatus = statusDto.RaceStatus;

            await _raceDataRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}

using FluentResults;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update.RaceDetails
{
    public class UpdateRaceDetailsService : IUpdateRaceDetailsService
    {
        private readonly IRaceDataRepository _raceDataRepository;
        private readonly ILogger<UpdateRaceDetailsService> _logger;
        private readonly IValidator<UpdateRaceDetailsDto> _validator;

        public UpdateRaceDetailsService(IRaceDataRepository raceDataRepository,
ILogger<UpdateRaceDetailsService> logger,
IValidator<UpdateRaceDetailsDto> validator)
        {
            _raceDataRepository = raceDataRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> UpdateRaceDetails(UpdateRaceDetailsDto raceDetailsDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateDtoAsync(raceDetailsDto, cancellationToken);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

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
    }
}

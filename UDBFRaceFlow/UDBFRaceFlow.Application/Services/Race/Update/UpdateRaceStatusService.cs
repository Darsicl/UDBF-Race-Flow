using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Update
{
    public class UpdateRaceStatusService : IUpdateRaceStatusService
    {
        private readonly IRaceDataRepository _raceDataRepository;
        private readonly ILogger<UpdateRaceStatusService> _logger;
        private readonly IValidator<UpdateStatusDto> _validator;

        public UpdateRaceStatusService(IRaceDataRepository raceDataRepository, ILogger<UpdateRaceStatusService> logger, IValidator<UpdateStatusDto> validator)
        {
            _raceDataRepository = raceDataRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result> UpdateRaceStatus(UpdateStatusDto statusDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateDtoAsync(statusDto, cancellationToken);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

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

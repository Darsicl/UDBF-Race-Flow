using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Update;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Update;

namespace UDBFRaceFlow.Application.Services.Race.Update.RaceDelay
{
    public class UpdateRaceDelayService : IUpdateRaceDelayService
    {
        private readonly IRaceDataRepository _raceDataRepository;
        private readonly ILogger<UpdateRaceDelayService> _logger;
        private readonly IValidator<RaceDelayDto> _validator;
        public UpdateRaceDelayService(ILogger<UpdateRaceDelayService> logger, IRaceDataRepository raceDataRepository, IValidator<RaceDelayDto> validator)
        {
            _logger = logger;
            _raceDataRepository = raceDataRepository;
            _validator = validator;
        }

        public async Task<Result> UpdateRaceDelay(RaceDelayDto raceDelayDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateDtoAsync(raceDelayDto, cancellationToken);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }
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


    }
}

using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.RaceSystems.SystemLong
{
    public class SystemLongGenerator : ISystemGenerator
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly IRaceDataRepository _raceDataRepository;
        private readonly ILogger<SystemLongGenerator> _logger;
        private readonly SystemLongDtoValidator _validator;

        public SystemLongGenerator(IRaceCategoryRepository raceCategoryRepository, ILogger<SystemLongGenerator> logger, SystemLongDtoValidator validator, IRaceDataRepository raceDataRepository)
        {
            _raceCategoryRepository = raceCategoryRepository;
            _logger = logger;
            _validator = validator;
            _raceDataRepository = raceDataRepository;
        }

        public bool ApplyParametrs(int raceType, RaceSystem raceSystems)
        {
            return raceSystems == RaceSystem.Long && raceType == 1;
        }
        public async Task<Result> BuildGridAsync(CreateFullGridDto fullGridDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(fullGridDto, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMsg = string.Join(",", validationResult.Errors.Select(v => v.ErrorMessage));
                _logger.LogWarning(string.Format(Messages.Error_ValidationFailed, fullGridDto.RaceSystem, errorMsg));
                return Result.Fail(new Error(errorMsg));
            }

            var category = fullGridDto.Adapt<RaceCategory>();

            List<RaceCreationDto> sortedRaces = fullGridDto.Races
                .OrderBy(s => s.RaceType)
                .ThenBy(s => s.SequenceNumber)
                .ToList();

            foreach (var raceDto in sortedRaces)
            {
                var race = raceDto.Adapt<RaceData>();

                race.RaceStatus = RaceStatus.Scheduled;

                race.CategoryId = category.Id;

                race.OriginalDateTime = race.RaceTime;

                foreach (var lane in race.Lanes)
                {
                    lane.RaceId = race.Id;
                }

                category.Races.Add(race);
            }

            var existingRaces = await _raceDataRepository.GetAllRacesAsync(cancellationToken);
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

        public Task<Result> BuildSemifinalAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Ok());
        }
        public Task<Result> BuildFinalAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}

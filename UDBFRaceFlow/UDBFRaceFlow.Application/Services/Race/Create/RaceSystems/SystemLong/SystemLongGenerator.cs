using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto.Request.Race.Create;
using UDBFRaceFlow.Application.Extensions.ValidatorExtensions;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts.Race.Create;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.Race.Create.RaceSystems.SystemLong
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
        public async Task<Result> BuildGridAsync(CreateCategoryDto fullGridDto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateDtoAsync(fullGridDto, cancellationToken);

            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var category = fullGridDto.Adapt<RaceCategory>();

            List<CreateRaceDto> sortedRaces = fullGridDto.Races
                .OrderBy(s => s.RaceType)
                .ThenBy(s => s.SequenceNumber)
                .ToList();

            foreach (var raceDto in sortedRaces)
            {
                var race = raceDto.Adapt<RaceData>();

                race.RaceStatus = RaceStatus.Scheduled;
                race.OriginalDateTime = race.RaceTime;

                foreach (var laneDto in race.Lanes)
                {
                    var lane = laneDto.Adapt<LaneData>();
                    race.AddLane(lane);
                }

                category.AddRace(race);
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

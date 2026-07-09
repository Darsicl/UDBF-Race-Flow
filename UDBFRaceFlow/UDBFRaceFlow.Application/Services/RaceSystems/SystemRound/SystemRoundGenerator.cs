using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.RaceSystems.SystemRound
{
    public class SystemRoundGenerator : ISystemGenerator
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly IRaceDataRepository _raceRepository;
        private readonly ILogger<SystemRoundGenerator> _logger;
        private readonly IEnumerable<IRoundGenerator> _generators;
        private readonly SystemRoundDtoValidator _validator;

        public SystemRoundGenerator(IRaceCategoryRepository raceCategoryRepository, ILogger<SystemRoundGenerator> logger, IEnumerable<IRoundGenerator> generators, IRaceDataRepository raceRepository, SystemRoundDtoValidator validator)
        {
            _raceCategoryRepository = raceCategoryRepository;
            _logger = logger;
            _generators = generators;
            _raceRepository = raceRepository;
            _validator = validator;
        }

        public bool ApplyParametrs(int SystemType, Domain.Enums.RaceSystem raceSystems)
        {
            return raceSystems == Domain.Enums.RaceSystem.Round && SystemType == 1;
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

            var category = fullGridDto.Adapt<RaceCategory>();

            _logger.LogInformation(Messages.Info_StartGeneratingGrid, category.Id);

            List<CreateRaceDto> sortRaces = fullGridDto.Races
                .OrderBy(s => s.RaceType)
                .ThenBy(s => s.SequenceNumber)
                .ToList();

            foreach (var raceDto in sortRaces)
            {
                var race = raceDto.Adapt<RaceData>();

                race.RaceStatus = RaceStatus.Scheduled;
                race.OriginalDateTime = race.RaceTime;

                foreach (var laneDto in raceDto.Lanes)
                {
                    var lane = laneDto.Adapt<LaneData>();
                    race.AddLane(lane);
                }

                category.AddRace(race);
            }

            var countOfTeams = category.Races
                .Where(c => c.RaceType == RaceType.Heat)
                .SelectMany(c => c.Lanes)
                .Count();

            var generator = _generators.FirstOrDefault(g => g.ApplyParametrs(countOfTeams));

            if (generator is null)
            {
                string errorMsg = string.Format(Messages.Error_RaceIsNull, countOfTeams);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            generator.CreateRestRound(category);

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

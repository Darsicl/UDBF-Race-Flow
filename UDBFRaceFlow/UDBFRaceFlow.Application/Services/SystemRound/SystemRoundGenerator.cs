using FluentResults;
using Mapster;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Dto;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services.SystemRound
{
    public class SystemRoundGenerator : ISystemGenerator
    {
        private readonly IRaceCategoryRepository _raceCategoryRepository;
        private readonly IRaceDataRepository _raceRepository;
        private readonly ILogger<SystemRoundGenerator> _logger;
        private readonly IEnumerable<IRoundGenerator> _generators;

        public SystemRoundGenerator(IRaceCategoryRepository raceCategoryRepository, ILogger<SystemRoundGenerator> logger, IEnumerable<IRoundGenerator> generators, IRaceDataRepository raceRepository)
        {
            _raceCategoryRepository = raceCategoryRepository;
            _logger = logger;
            _generators = generators;
            _raceRepository = raceRepository;
        }

        public bool ApplyParametrs(int SystemType, RaceSystems raceSystems)
        {
            return raceSystems == RaceSystems.Round && SystemType == 1;
        }
        public async Task<Result> BuildGrid(CreateFullGridDto fullGridDto)
        {
            var category = fullGridDto.Adapt<RaceCategory>();

            _logger.LogInformation(Messages.Info_StartGeneratingGrid, category.Id);

            List<RaceCreationDto> sortRaces = fullGridDto.Races
                .OrderBy(s => s.RaceType)
                .ThenBy(s => s.SequenceNumber)
                .ToList();

            foreach (var raceDto in sortRaces)
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


            var existingRaces = await _raceRepository.GetAllRacesAsync();
            var allRaces = existingRaces.Concat(category.Races).ToList();
            var check = CheckIntervalTimeExtension.CheckInterval(allRaces);

            if (!check)
            {
                string errorMsg = string.Format(Messages.Error_CheckIntervalFail, category.CategoryName);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _raceCategoryRepository.AddAsync(category);
            await _raceCategoryRepository.SaveChangesAsync();

            _logger.LogInformation(Messages.Info_FinishGenerateGrid, category.Id);

            return Result.Ok();

        }
        public Task<Result> BuildSemifinal(Guid categoryId)
        {
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> BuildFinal(Guid categoryId)
        {
            return Task.FromResult(Result.Ok());
        }

    }
}

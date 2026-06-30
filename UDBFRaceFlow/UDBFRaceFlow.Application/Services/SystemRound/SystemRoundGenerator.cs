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
        private readonly IRaceCategoryRepository _raceRepository;
        private readonly ILogger<SystemRoundGenerator> _logger;
        private readonly IEnumerable<IRoundGenerator> _generators;

        public SystemRoundGenerator(IRaceCategoryRepository raceRepository, ILogger<SystemRoundGenerator> logger, IEnumerable<IRoundGenerator> generators)
        {
            _raceRepository = raceRepository;
            _logger = logger;
            _generators = generators;
        }

        public bool ApplyParametrs(int SystemType, RaceSystems raceSystems)
        {
            return raceSystems == RaceSystems.Round && SystemType == 1;
        }
        public async Task<Result> BuildGrid(CreateFullGridDto fullGridDto)
        {
            var category = fullGridDto.Adapt<RaceCategory>();

            _logger.LogInformation(Messages.Info_StartGeneratingGrid, category.Id);

            category.Races = new List<RaceData>();

            List<RaceCreationDto> sortRaces = fullGridDto.Races
                .OrderBy(s => s.RaceType)
                .ThenBy(s => s.SequenceNumber)
                .ToList();

            foreach (var raceDto in sortRaces)
            {
                var race = raceDto.Adapt<RaceData>();

                race.RaceStatus = RaceStatus.Scheduled;

                race.CategoryId = category.Id;

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

            await _raceRepository.AddAsync(category);
            await _raceRepository.SaveChangesAsync();

            _logger.LogInformation(Messages.Info_FinishGenerateGrid, category.Id);

            return Result.Ok();

        }
        public Task<Result> BuildSemifinal(Guid categoryId)
        {
            return Task.FromResult(Result.Ok());
        }

        public async Task<Result> BuildFinal(Guid categoryId)
        {
            var category = await _raceRepository.GetCategoryWithRacesAndLanesAsync(categoryId);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var finalFinished = category.Races
                .Where(f => f.RaceType == RaceType.Final)
                .All(f => f.RaceStatus == RaceStatus.Finished);

            if (!finalFinished)
            {
                return Result.Ok();
            }

            var finalLeaderBoard = category.Races
                .SelectMany(f => f.Lanes)
                .Where(f => f.FinishTime > TimeSpan.Zero)
                .GroupBy(f => f.TeamId)
                .Select(g => new
                {
                    TeamId = g.Key,
                    TotalTime = TimeSpan.FromMilliseconds(g.Sum(f => f.FinishTime.TotalMilliseconds))
                })
                .OrderBy(f => f.TotalTime)
                .ToList();

            return Result.Ok();
        }

    }
}

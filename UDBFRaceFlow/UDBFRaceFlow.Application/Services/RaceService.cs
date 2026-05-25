using FluentResults;
using Microsoft.Extensions.Logging;
using UDBFRaceFlow.Application.Interfaces;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;
using UDBFRaceFlow.Domain.Resources;

namespace UDBFRaceFlow.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepo;
        private readonly IEnumerable<ISystemGenerator> _generators;
        private readonly ILogger<RaceService> _logger;
        public RaceService(IRaceRepository raceRepo, IEnumerable<ISystemGenerator> generators, ILogger<RaceService> logger)
        {
            _raceRepo = raceRepo;
            _generators = generators;
            _logger = logger;
        }

        public async Task<Result> CheckFinishOfHeats(Guid categoryId)
        {
            _logger.LogInformation("Method that cheking, is heats finished, begins");

            RaceCategory category = await _raceRepo.GetCategory(categoryId);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            bool finishedRaces = category.Races
                .Where(f => f.RaceType == RaceType.Heat)
                .All(f => f.RaceStatus == RaceStatus.Finished);

            if (finishedRaces)
            {
                ISystemGenerator? systemForSemis = _generators.FirstOrDefault(g => g.raceSystem == category.RaceSystem);

                if (systemForSemis is null)
                {
                    string errorMsg = string.Format(Messages.Error_MethodNotFound, nameof(systemForSemis));
                    _logger.LogError(errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }

                await systemForSemis.BuildSemifinal(categoryId);

                _logger.LogInformation("Semis builded succesfully");

                return Result.Ok();
            }

            _logger.LogInformation("Not all heats finished");

            return Result.Ok();
        }

        public async Task<Result> CheckFinishOfSemis(Guid categoryId)
        {
            _logger.LogInformation("Method that cheking, is semis finished, begins");

            RaceCategory category = await _raceRepo.GetCategory(categoryId);

            if (category is null)
            {
                string errorMsg = string.Format(Messages.Error_EntityWithIdNotFound, nameof(RaceCategory), categoryId);
                _logger.LogError(errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            bool finishedRaces = category.Races
                .Where(f => f.RaceType == RaceType.Semifinal)
                .All(f => f.RaceStatus == RaceStatus.Finished);

            if (finishedRaces)
            {
                ISystemGenerator? systemForSemis = _generators.FirstOrDefault(g => g.raceSystem == category.RaceSystem);

                if (systemForSemis is null)
                {
                    string errorMsg = string.Format(Messages.Error_MethodNotFound, nameof(systemForSemis));
                    _logger.LogError(errorMsg);
                    return Result.Fail(new Error(errorMsg));
                }

                await systemForSemis.BuildFinal(categoryId);

                _logger.LogInformation("Finals builded succesfully");

                return Result.Ok();
            }

            _logger.LogInformation("Not all semis finished");

            return Result.Ok();
        }
    }
}

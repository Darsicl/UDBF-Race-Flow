using UDBFRaceFlow.Application.Interfaces;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepo;
        private readonly IEnumerable<ISystemGenerator> _generators;

        public RaceService(IRaceRepository raceRepo, IEnumerable<ISystemGenerator> generators)
        {
            _raceRepo = raceRepo;
            _generators = generators;
        }

        public async Task CheckFinishOfHeats(Guid categoryId)
        {
            RaceCategory category = await _raceRepo.GetCategory(categoryId);

            bool finishedRaces = category.Races
                .Where(f => f.RaceType == RaceType.Heat)
                .All(f => f.RaceStatus == RaceStatus.Finished);

            if (finishedRaces)
            {
                var systemForSemis = _generators.FirstOrDefault(g => g.raceSystem == category.RaceSystem)
            }
        }

        public async Task CheckFinishOfSemis(Guid categoryId)
        {
            RaceCategory category = await _raceRepo.GetCategory(categoryId);

            bool finishedRaces = category.Races
                .Where(f => f.RaceType == RaceType.Semifinal)
                .Any(f => f.RaceStatus == RaceStatus.Finished);
        }
    }
}

using UDBFRaceFlow.Application.Interfaces;
using UDBFRaceFlow.Application.Interfaces.ServiceContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Application.Services
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepo;

        public RaceService(IRaceRepository raceRepo)
        {
            _raceRepo = raceRepo;
        }

        public async Task CheckFinishOfHeats(Guid categoryId)
        {
            RaceCategory category = await _raceRepo.GetCategory(categoryId);

            var finishedRaces = category.Races
                .Where(f => f.RaceType == RaceType.Heat)
                .All(f => f.RaceStatus == RaceStatus.Finished);

            if (finishedRaces)
            {
                BuildSemifinal
            }
        }

        public async Task CheckFinishOfSemis(Guid categoryId)
        {
            RaceCategory category = await _raceRepo.GetCategory(categoryId);

            var finishedRaces = category.Races
                .Where(f => f.RaceType == RaceType.Semifinal)
                .Any(f => f.RaceStatus == RaceStatus.Finished);
        }
    }
}

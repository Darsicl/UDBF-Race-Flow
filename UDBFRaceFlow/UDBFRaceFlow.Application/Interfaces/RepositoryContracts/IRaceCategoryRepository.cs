using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.RepositoryContracts
{
    public interface IRaceCategoryRepository : IRaceRepository<RaceCategory>
    {
        Task<RaceCategory?> GetCategoryWithRacesAndLanesAsync(Guid id);
    }
}

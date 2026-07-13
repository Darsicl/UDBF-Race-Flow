using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.RepositoryContracts
{
    public interface IRaceCategoryRepository : IBaseRepository<RaceCategory>
    {
        Task<RaceCategory?> GetCategoryWithRacesAndLanesAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> IsCategoryUnique(RaceCategory category, CancellationToken cancellationToken);
    }
}

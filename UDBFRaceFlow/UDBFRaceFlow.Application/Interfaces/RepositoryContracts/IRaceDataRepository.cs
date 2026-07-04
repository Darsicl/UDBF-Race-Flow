using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.RepositoryContracts
{
    public interface IRaceDataRepository : IRaceRepository<RaceData>
    {
        Task<List<RaceData>> GetAllRacesAsync(CancellationToken cancellationToken);
    }
}

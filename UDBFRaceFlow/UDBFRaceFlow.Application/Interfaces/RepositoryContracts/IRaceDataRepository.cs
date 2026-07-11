using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.RepositoryContracts
{
    public interface IRaceDataRepository : IRaceRepository<RaceData>
    {
        Task<List<RaceData>> GetAllRacesAsync(CancellationToken cancellationToken);
        Task<RaceData?> GetRaceWithLanes(Guid raceId, CancellationToken cancellationToken);
        Task<bool> IsRaceNumberUnique(int raceNumber, CancellationToken cancellationToken);
        Task<bool> IsRaceDateUnique(DateTime raceTime, CancellationToken cancellationToken);

    }
}

using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces.RepositoryContracts
{
    public interface IRaceDataRepository : IBaseRepository<RaceData>
    {
        Task<List<RaceData>> GetAllRacesAsync(CancellationToken cancellationToken);
        Task<RaceData?> GetRaceWithLanesAsync(Guid raceId, CancellationToken cancellationToken);
        Task<bool> IsRaceNumberUniqueAsync(int raceNumber, Guid raceId, CancellationToken cancellationToken);
        Task<bool> IsRaceNumberUniqueAsync(int raceNumber, CancellationToken cancellationToken);
        Task<bool> IsRaceDateUniqueAsync(DateTime raceTime, Guid raceId, CancellationToken cancellationToken);
        Task<bool> IsRaceDateUniqueAsync(DateTime raceTime, CancellationToken cancellationToken);
        Task<bool> IsRacesHasActiveTeam(Guid TeamId, CancellationToken cancellationToken);
        Task<List<RaceData>> GetRacesByDayForDelayAsync(DateTime date, CancellationToken cancellationToken);

    }
}

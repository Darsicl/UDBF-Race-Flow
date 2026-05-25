using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces
{
    public interface IRaceRepository
    {
        Task AddAsync(RaceData race);
        Task SaveChangesAsync();
    }
}
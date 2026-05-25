using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Application.Interfaces
{
    public interface IRaceRepository
    {
        Task AddCategoryAsync(RaceCategory category);

        Task AddRaceAsync(RaceData race);

        Task AddLaneAsync(LaneData lane);

        Task<RaceData> GetRace(Guid raceId);

        Task<RaceCategory> GetCategory(Guid categoryId);

        Task SaveChangesAsync();
    }
}
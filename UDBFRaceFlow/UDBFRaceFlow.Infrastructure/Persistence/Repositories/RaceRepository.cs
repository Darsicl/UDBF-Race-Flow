using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly RaceDbContext _context;

        public RaceRepository(RaceDbContext context)
        {
            _context = context;
        }

        public async Task AddCategoryAsync(RaceCategory category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task AddRaceAsync(RaceData race)
        {
            await _context.Races.AddAsync(race);
        }

        public async Task AddLaneAsync(LaneData lane)
        {
            await _context.Lanes.AddAsync(lane);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<RaceData> GetRace(Guid raceId)
        {
            return await _context.Races
                .Include(r => r.Lanes)
                .FirstOrDefaultAsync(r => r.Id == raceId);
        }

        public async Task<RaceCategory> GetCategory(Guid categoryId)
        {
            return await _context.Categories
                .Include(c => c.Races)
                .ThenInclude(c => c.Lanes)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class RaceCategoryRepository : RaceRepository<RaceCategory>, IRaceCategoryRepository
    {
        private readonly RaceDbContext _context;
        public RaceCategoryRepository(RaceDbContext raceDbContext) : base(raceDbContext)
        {
            _context = raceDbContext;
        }
        public async Task<RaceCategory?> GetCategoryWithRacesAndLanesAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Races)
                .ThenInclude(c => c.Lanes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

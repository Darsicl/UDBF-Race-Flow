using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class RaceCategoryRepository : BaseRepository<RaceCategory>, IRaceCategoryRepository
    {
        private readonly RaceDbContext _context;
        public RaceCategoryRepository(RaceDbContext raceDbContext) : base(raceDbContext)
        {
            _context = raceDbContext;
        }
        public async Task<RaceCategory?> GetCategoryWithRacesAndLanesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.Races)
                .ThenInclude(c => c.Lanes)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> IsCategoryUnique(RaceCategory category, CancellationToken cancellationToken)
        {
            return await _context.Categories.AnyAsync(r => r.CategoryName == category.CategoryName && r.Id != category.Id, cancellationToken);
        }
    }
}

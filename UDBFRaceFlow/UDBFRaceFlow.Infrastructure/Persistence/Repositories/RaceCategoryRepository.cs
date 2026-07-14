using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

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

        public async Task<bool> IsCategoryActiveAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Races.AnyAsync(c => c.CategoryId == categoryId && c.RaceStatus != RaceStatus.Scheduled, cancellationToken);
        }

        public async Task<bool> IsCategoryUniqueAsync(RaceCategory category, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(r => r.CategoryName == category.CategoryName && r.Id != category.Id, cancellationToken);
        }


    }
}

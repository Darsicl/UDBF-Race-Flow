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
        }


        public async Task<RaceCategory?> GetCategoryWithRacesAndLanesAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Races)
                .ThenInclude(c => c.Lanes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        //public async Task<RaceData> GetRace(Guid raceId)
        //{
        //    return await _context.Races
        //        .Include(r => r.Lanes)
        //        .FirstOrDefaultAsync(r => r.Id == raceId);
        //}

        //public async Task<RaceCategory> GetCategory(Guid categoryId)
        //{
        //    return await _context.Categories
        //        .Include(c => c.Races)
        //        .ThenInclude(c => c.Lanes)
        //        .FirstOrDefaultAsync(c => c.Id == categoryId);
        //}

    }
}

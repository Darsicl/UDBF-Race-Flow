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

        public async Task AddAsync(RaceData race)
        {
            await _context.Races.AddAsync(race);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

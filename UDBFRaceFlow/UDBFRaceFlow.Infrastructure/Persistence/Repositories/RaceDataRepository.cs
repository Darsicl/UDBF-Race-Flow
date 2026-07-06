using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class RaceDataRepository : RaceRepository<RaceData>, IRaceDataRepository
    {
        private readonly RaceDbContext _context;
        public RaceDataRepository(RaceDbContext raceDbContext) : base(raceDbContext)
        {
            _context = raceDbContext;
        }

        public async Task<List<RaceData>> GetAllRacesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Races
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<bool> IsRaceDateUnique(DateTime raceTime, CancellationToken cancellationToken)
        {
            return !await _context.Races.AnyAsync(r => r.RaceTime == raceTime, cancellationToken);
        }

        public async Task<bool> IsRaceNumberUnique(int raceNumber, CancellationToken cancellationToken)
        {
            return !await _context.Races.AnyAsync(r => r.RaceNumber == raceNumber, cancellationToken);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

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

        public async Task<List<RaceData>> GetRacesByDayForDelayAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.Races
                .Where(c => c.OriginalDateTime == date && (c.RaceStatus == RaceStatus.Scheduled || c.RaceStatus == RaceStatus.GetReady))
                .ToListAsync();
        }

        public async Task<RaceData?> GetRaceWithLanesAsync(Guid raceId, CancellationToken cancellationToken = default)
        {
            return await _context.Races
                .Include(r => r.Lanes)
                .FirstOrDefaultAsync(r => r.Id == raceId, cancellationToken);
        }

        public async Task<bool> IsRaceDateUniqueAsync(DateTime raceTime, CancellationToken cancellationToken)
        {
            return !await _context.Races.AnyAsync(r => r.RaceTime == raceTime, cancellationToken);
        }

        public async Task<bool> IsRaceNumberUniqueAsync(int raceNumber, CancellationToken cancellationToken)
        {
            return !await _context.Races.AnyAsync(r => r.RaceNumber == raceNumber, cancellationToken);
        }
    }
}

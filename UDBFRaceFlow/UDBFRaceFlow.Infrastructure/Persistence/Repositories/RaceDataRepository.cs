using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class RaceDataRepository : BaseRepository<RaceData>, IBaseRepository<RaceData>
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
            return await _context.Races.AnyAsync(r => r.RaceTime == raceTime, cancellationToken);
        }

        public async Task<bool> IsRaceDateUniqueAsync(DateTime raceTime, Guid raceId, CancellationToken cancellationToken)
        {
            return await _context.Races.AnyAsync(r => r.OriginalDateTime == raceTime && r.Id != raceId, cancellationToken);
        }

        public async Task<bool> IsRaceNumberUniqueAsync(int raceNumber, Guid raceId, CancellationToken cancellationToken)
        {
            return await _context.Races.AnyAsync(r => r.RaceNumber == raceNumber && r.Id != raceId, cancellationToken);
        }

        public async Task<bool> IsRaceNumberUniqueAsync(int raceNumber, CancellationToken cancellationToken)
        {
            return await _context.Races.AnyAsync(r => r.RaceNumber == raceNumber, cancellationToken);
        }

        public async Task<bool> HasActiveRacesWithTeamAsync(Guid TeamId, CancellationToken cancellationToken)
        {
            return await _context.Races.AnyAsync(r => r.Lanes.Any(l => l.TeamId == TeamId), cancellationToken);
        }
    }
}

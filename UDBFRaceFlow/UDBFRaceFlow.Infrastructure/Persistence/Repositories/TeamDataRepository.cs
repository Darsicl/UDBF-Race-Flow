using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Application.Interfaces.RepositoryContracts;
using UDBFRaceFlow.Domain.Entities.Team;

namespace UDBFRaceFlow.Infrastructure.Persistence.Repositories
{
    public class TeamDataRepository : BaseRepository<TeamData>, ITeamDataRepository
    {
        private readonly RaceDbContext _context;

        public TeamDataRepository(RaceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsTeamExistsAsync(string TeamName, CancellationToken cancellationToken)
        {
            return await _context.Teams.AnyAsync(t => t.Name == TeamName, cancellationToken);
        }

        public async Task<bool> IsTeamExistsAsync(string TeamName, Guid teamId, CancellationToken cancellationToken)
        {
            return await _context.Teams.AnyAsync(r => r.Name == TeamName && r.Id != teamId, cancellationToken);
        }
    }
}

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
    }
}

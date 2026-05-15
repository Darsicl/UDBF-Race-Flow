using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Domain.Entities.Race;
using UDBFRaceFlow.Domain.Entities.Team;

namespace UDBFRaceFlow.Infrastructure.Persistence
{
    public class RaceDbContext : DbContext
    {
        public RaceDbContext(DbContextOptions<RaceDbContext> options) : base(options) { }

        public DbSet<RaceData> Races { get; set; }
        public DbSet<RaceEntry> RaceEntries { get; set; }
        public DbSet<TeamData> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RaceDbContext).Assembly);


            base.OnModelCreating(modelBuilder);
        }
    }
}

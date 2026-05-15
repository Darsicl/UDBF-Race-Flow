using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Configurations
{
    public class RaceEntryConfiguration : IEntityTypeConfiguration<RaceEntry>
    {
        public void Configure(EntityTypeBuilder<RaceEntry> builder)
        {
            builder.HasKey(r => r.Id);

            builder
                .HasOne(r => r.Race)
                .WithMany(r => r.RaceEntries)
                .HasForeignKey(r => r.RaceId);

            builder
                .HasOne(r => r.Team)
                .WithMany(r => r.RaceEntries)
                .HasForeignKey(r => r.TeamId);
        }
    }
}

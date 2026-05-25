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

            builder.HasIndex(r => r.RaceId);

            builder
                .HasOne(r => r.Team)
                .WithMany(r => r.RaceEntries)
                .HasForeignKey(r => r.TeamId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Configurations
{
    public class RaceDataConfiguration : IEntityTypeConfiguration<RaceData>
    {
        public void Configure(EntityTypeBuilder<RaceData> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasIndex(r => r.RaceNumber)
                .IsUnique();

            builder
                .HasMany(r => r.RaceEntries)
                .WithOne(r => r.Race);
        }
    }
}

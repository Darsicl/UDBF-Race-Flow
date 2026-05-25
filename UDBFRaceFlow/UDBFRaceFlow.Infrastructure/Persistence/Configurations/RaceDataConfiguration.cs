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

            builder.HasIndex(r => r.CategoryId);

            builder
                .HasMany(r => r.RaceEntries)
                .WithOne(r => r.Race)
                .HasForeignKey(r => r.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(r => r.Category)
                .WithMany(r => r.Races)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UDBFRaceFlow.Domain.Entities.Team;

namespace UDBFRaceFlow.Infrastructure.Persistence.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<TeamData>
    {
        public void Configure(EntityTypeBuilder<TeamData> builder)
        {
            builder.HasKey(r => r.Id);

            builder
                .HasMany(r => r.RaceEntries)
                .WithOne(r => r.Team);

            builder.HasIndex(r => r.Name)
                .IsUnique();
        }
    }
}

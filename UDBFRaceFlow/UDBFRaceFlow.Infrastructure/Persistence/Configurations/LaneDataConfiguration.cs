using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Configurations
{
    public class LaneDataConfiguration : IEntityTypeConfiguration<LaneData>
    {
        public void Configure(EntityTypeBuilder<LaneData> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasIndex(r => r.RaceId);

            builder
                .HasOne(r => r.Team)
                .WithMany(r => r.Lanes)
                .HasForeignKey(r => r.TeamId);
        }
    }
}

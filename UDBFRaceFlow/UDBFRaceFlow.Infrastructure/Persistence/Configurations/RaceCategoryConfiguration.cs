using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UDBFRaceFlow.Domain.Entities.Race;

namespace UDBFRaceFlow.Infrastructure.Persistence.Configurations
{
    public class RaceCategoryConfiguration : IEntityTypeConfiguration<RaceCategory>
    {
        public void Configure(EntityTypeBuilder<RaceCategory> builder)
        {
            builder.HasKey(r => r.Id);
        }
    }
}

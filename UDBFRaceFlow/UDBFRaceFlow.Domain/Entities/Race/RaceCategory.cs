using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Domain.Entities.Race
{
    public class RaceCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public BoatSize BoatSize { get; set; }

        [Range(200, 2000)]
        public int Distance { get; set; }

        [Required]
        public GenderCategory GenderCategory { get; set; }

        public List<RaceData> Races { get; set; } = new List<RaceData>();

    }
}

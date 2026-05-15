using System.ComponentModel.DataAnnotations;
using UDBFRaceFlow.Domain.Enums;

namespace UDBFRaceFlow.Domain.Entities.Race
{
    public class RaceData
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int RaceNumber { get; set; }

        [Required]
        [MaxLength(7)]
        public string BoatSize { get; set; } = string.Empty;

        [Range(200, 2000)]
        public int Distance { get; set; }

        [Required]
        public GenderCategory GenderCategory { get; set; }

        [Required]
        public RaceStatus RaceStatus { get; set; }

        [Required]
        public RaceType RaceType { get; set; }

        public List<RaceEntry> RaceEntries { get; set; } = new List<RaceEntry>();
    }
}
